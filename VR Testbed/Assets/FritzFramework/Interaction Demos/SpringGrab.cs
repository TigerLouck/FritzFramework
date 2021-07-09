using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class SpringGrab : MonoBehaviour
	{
		private InteractableObject controllingInteractable;
		private Rigidbody RB;

		public GameObject templateConfigJoint;
		public float dominantHandTorqueLength;
		// MUST BE NORMALIZED
		public Vector3 objectForward;
		public Vector3 objectUp;

		// Exposed for debugging, not to be assigned, clean later
		public bool oneHandGrab;
		public bool twoHandGrab;
		public InteractabilityLinker firstHandLink;
		public SpringJoint firstHandJoint;
		public InteractabilityLinker secondHandLink;
		public SpringJoint secondHandJoint;

		public ConfigurableJoint SingleHandJoint;


		public float springDamper;
		public float springForce;

		// Springs do not enforce angular limits, so if the user holds via spring an object one-handed
		// it hangs like a pendulum. A much more complicated configurable joint is required in that case.
		// However in two-handed mode the second hand needs to be a spring joint, because the second hand does not control rotation
		// But that means the primary hand can only control roll, and so the primary hand must be determined somehow, in addition 
		// to the more complicated configuration process.

		// Start is called before the first frame update
		void Start()
		{
			controllingInteractable = GetComponent<InteractableObject>();
			RB = GetComponent<Rigidbody>();
			if (templateConfigJoint?.GetComponent<ConfigurableJoint>() == null)
				Debug.LogError("There's no configurable joint in the template object, this script is about to break!");

			if (controllingInteractable == null)
				Debug.LogError("There's no Interactable on this object, this script will not do anything!");
		}

		private void FixedUpdate()
		{
			//Apply a force at a position determined by the orientation of the dominant hand
			//Will need a dominant hand toggle at some point
			if (twoHandGrab)
			{
				Vector3 leverGun = transform.rotation * objectUp * dominantHandTorqueLength;
				Transform hand = InputUtils.GetTransformFromInSource(SteamVR_Input_Sources.RightHand);
				Vector3 leverHand = Vector3.ProjectOnPlane(hand.up, transform.rotation * objectForward).normalized * dominantHandTorqueLength;
				RB.AddForceAtPosition(leverHand - leverGun, leverGun + transform.position);
				RB.AddForceAtPosition(-(leverHand - leverGun), -leverGun + transform.position);
				Debug.DrawRay(transform.position, leverGun);
				Debug.DrawRay(hand.position, leverHand);
				Debug.DrawRay(transform.position + leverGun, leverHand - leverGun, Color.cyan);

				//Antigravity force
				RB.AddForceAtPosition(Vector3.up * 9.8f * RB.mass/2, firstHandJoint.connectedAnchor + transform.position);
				//RB.AddForceAtPosition(Vector3.up * 9.8f * RB.mass / 2, secondHandJoint.connectedAnchor + transform.position);

				//Those Spring joints need to move to stay stable
				//This is one of the things where options for dominant hand would be needed
				//The moving joint needs to be the non-dominant hand
				if (firstHandLink.thisSource != SteamVR_Input_Sources.RightHand)
				{
					Vector3 handSlide = transform.InverseTransformDirection(Vector3.Project(firstHandJoint.connectedBody.transform.TransformPoint(firstHandJoint.connectedAnchor) - transform.position, transform.rotation * objectForward));
					Debug.DrawRay(transform.position, transform.rotation * handSlide, Color.blue, .1f);
					firstHandJoint.anchor = handSlide + new Vector3(
						(1 - objectForward.x) * firstHandJoint.anchor.x,
						(1 - objectForward.y) * firstHandJoint.anchor.y,
						(1 - objectForward.z) * firstHandJoint.anchor.z
					);
				}
				else
				{
					Vector3 handSlide = transform.InverseTransformDirection(Vector3.Project(secondHandJoint.connectedBody.transform.TransformPoint(secondHandJoint.connectedAnchor) - transform.position, transform.rotation * objectForward));
					Debug.DrawRay(transform.position, transform.rotation * handSlide, Color.blue, .1f);
					secondHandJoint.anchor = handSlide + new Vector3(
						(1 - objectForward.x) * secondHandJoint.anchor.x,
						(1 - objectForward.y) * secondHandJoint.anchor.y,
						(1 - objectForward.z) * secondHandJoint.anchor.z
					);
				}
			}
			if (oneHandGrab)
			{
				//Antigravity force for...stability? in a sense?
				RB.AddForceAtPosition(Vector3.up * 9.8f * RB.mass, SingleHandJoint.connectedAnchor + transform.position);
			}
		}

		//The meat of this system is everything below this line
		public virtual void InteractableLinkChanged(SteamVR_Input_Sources source, InteractabilityLinker hand, bool isConnecting)
		{
			if (isConnecting)
			{
				// currentLink being null indicates the first connection
				if (firstHandLink == null)
				{
					firstHandLink = hand;
				}
				else if (secondHandLink == null)
				{
					secondHandLink = hand;
				}

			}
			else
			{
				if (hand == firstHandLink)
				{
					firstHandLink = null;
				}
				else if (hand == secondHandLink)
				{
					secondHandLink = null;
				}
			}
		}

		// Update is called once per frame
		public virtual void ToggleGrab(SteamVR_Input_Sources source, bool value)
		{
			if (!oneHandGrab && !twoHandGrab) // in no-hand, any hand toggles, now in one-hand
			{
				oneHandGrab = true;
				Debug.Log("One-hand grab");
				controllingInteractable.LockLink(InputUtils.GetLinkerFromInSource(source));
				ConfigureForOneHand(InputUtils.GetLinkerFromInSource(source));
			}
			else if (oneHandGrab)
			{
				// in one-hand, if first hand toggles, now in no hand
				// disconnect one hand system, connect two hand if it's actually the second hand toggling
				if (InputUtils.GetTransformFromInSource(source) != SingleHandJoint.transform.parent) // in one-hand, second hand toggles, now in two hand
				{
					oneHandGrab = false;
					twoHandGrab = true;
					Debug.Log("Two-hand grab");
					ConfigureForTwoHand();
					controllingInteractable.LockLink(InputUtils.GetLinkerFromInSource(source));
				}
				else
				{
					Debug.Log("No-hand grab");
					controllingInteractable.UnlockLink(InputUtils.GetLinkerFromInSource(source));
					oneHandGrab = false;
					RB.useGravity = true;
				}
				// this happens second because the conditional depends on the joint
				ConfigureForRemoveOneHand();
			}
			else if (twoHandGrab) // in two-hand, any hand toggles, now in one hand
			{
				controllingInteractable.UnlockLink(InputUtils.GetLinkerFromInSource(source));
				oneHandGrab = true;
				twoHandGrab = false;
				Debug.Log("One-hand from two-hand grab");
				ConfigureForNotTwoHand();
				// goddamn long-ass line just to invert the source
				ConfigureForOneHand(InputUtils.GetLinkerFromInSource(source == SteamVR_Input_Sources.RightHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand));
			}
		}

		// in each of these configure methods, we handle physics and linker locking
		protected virtual void ConfigureForOneHand(InteractabilityLinker target)
		{
			//Instantiate the template joint as a child of the hand
			SingleHandJoint = Instantiate(templateConfigJoint, target.transform, false).GetComponent<ConfigurableJoint>();
			//capture and then point this joint at the  
			SingleHandJoint.connectedBody = RB;
		}

		protected virtual void ConfigureForTwoHand()
		{
			firstHandJoint = gameObject.AddComponent<SpringJoint>();
			firstHandJoint.damper = springDamper;
			firstHandJoint.spring = springForce;
			firstHandJoint.autoConfigureConnectedAnchor = true;
			firstHandJoint.connectedBody = firstHandLink.RB;
			firstHandJoint.anchor = transform.InverseTransformDirection(firstHandLink.transform.position - transform.position);

			secondHandJoint = gameObject.AddComponent<SpringJoint>();
			secondHandJoint.damper = springDamper;
			secondHandJoint.spring = springForce;
			secondHandJoint.autoConfigureConnectedAnchor = true;
			secondHandJoint.connectedBody = secondHandLink.RB;
			secondHandJoint.anchor = transform.InverseTransformDirection(secondHandLink.transform.position - transform.position);

			// If the base anchors are move, the autoconfig accounts for it, so we allow one config pass before handling
			firstHandJoint.autoConfigureConnectedAnchor = false;
			secondHandJoint.autoConfigureConnectedAnchor = false;
		}

		protected virtual void ConfigureForNotTwoHand()
		{
			Destroy(firstHandJoint);
			firstHandJoint = null;
			Destroy(secondHandJoint);
			secondHandJoint = null;
		}

		protected virtual void ConfigureForRemoveOneHand()
		{
			Destroy(SingleHandJoint.gameObject);
			SingleHandJoint = null;
		}
	}
}