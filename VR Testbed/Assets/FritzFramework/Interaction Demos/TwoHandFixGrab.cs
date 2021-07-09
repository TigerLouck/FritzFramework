using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class TwoHandFixGrab : FixedJointGrab
	{
		public GameObject TwoHandPivot;
		protected bool grabbed2 = false;
		protected SteamVR_Input_Sources grabber2 = SteamVR_Input_Sources.Any;
		protected InteractabilityLinker currentLink2 = null;
		protected Vector3 heldObjectLastPosition2 = new Vector3();
		protected Transform dominant;
		protected Rigidbody pivotRB;

		public override void InteractableLinkChanged(SteamVR_Input_Sources source, InteractabilityLinker hand, bool isConnecting)
		{
			if (isConnecting)
			{
				// currentLink being null indicates the first connection
				if (currentLink == null)
				{
					base.InteractableLinkChanged(source, hand, isConnecting);
				}
				else if (currentLink2 == null)
				{
					currentLink2 = hand;
				}

			}
			else
			{
				if (hand == currentLink)
				{
					base.InteractableLinkChanged(source, hand, isConnecting);
				} 
				else if (hand == currentLink2)
				{
					currentLink2 = null;
				}
			}
		}

		public override void ToggleGrab(SteamVR_Input_Sources source, bool value) // value will always be true, use with onDown events
		{
			// If we're one-handed and this is grabber, or no-handed, use base grab system
			if ((source == grabber && grabber2 == SteamVR_Input_Sources.Any) || !grabbed)
			{
				//Sets grabbed to true or false in here
				base.ToggleGrab(source, value);
				// Clean up after base if we were grabbed and aren't now
			}
			//We are grabbed with the first hand
			else if (grabbed)
			{
				// If we haven't grabbed with the second hand yet, handle disconnecting base grab to connect this
				if (!grabbed2)
				{
					controllingInteractable.LockLink(currentLink2);
					//In two hand mode, instead of attaching a fixed joint to the controller's rigidbody
					//we produce a phantom rigidbody on a pivot. we fix the pivot to the grabbable's body
					//and then drive the pivot directly
					grabber2 = source;
					grabbed2 = true;
					Destroy(joint);
					Destroy(pivotRB); // There's some kind of disorder somewhere than mandates this
					// Now handle the pivot
					pivotRB = TwoHandPivot.AddComponent<Rigidbody>();
					pivotRB.isKinematic = true; // We drive position 
					joint = TwoHandPivot.AddComponent<FixedJoint>();
					dominant = InputUtils.GetTransformFromInSource(grabber);
					// In the base, I get the source transform of the input because the link there is mostly a courtesy for FX
					// But in theory there's only two hands, so I can use the two links for their transforms
					// Average position, look rotation
					TwoHandPivot.transform.position = (currentLink.transform.position + currentLink2.transform.position) / 2;
					TwoHandPivot.transform.rotation = Quaternion.LookRotation(
						(currentLink.transform.position - currentLink2.transform.position).normalized,
						dominant.up //Link and Grab can be out of order
					);
					joint.connectedBody = GetComponent<Rigidbody>();
					// In attachment, the pivot becomes offset, so handle it
					joint.connectedAnchor = TwoHandPivot.transform.position - transform.position;
					joint.enablePreprocessing = false;
				}
				else
				{

					controllingInteractable.UnlockLink(currentLink2);
					//Completely let go, then run base grab
					//We need to figure out which hand is letting go and grab with the other
					grabber2 = SteamVR_Input_Sources.Any;
					// Whatever hand is letting go, the opposite must be holding on
					grabber = source == SteamVR_Input_Sources.LeftHand ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
					//grabbed is flipped in base, so account here
					grabbed = false;
					grabbed2 = false;
					Destroy(joint);
					joint = null;//this must be explicit cause unity is a funny guy
					Destroy(pivotRB);
					//Call base
					base.ToggleGrab(grabber, true);
				}
			}
		}

		protected override void Update()
		{
			//Update pivot
			if (grabbed && grabbed2)
			{
				TwoHandPivot.transform.position = (currentLink.transform.position + currentLink2.transform.position) / 2;
				TwoHandPivot.transform.rotation = Quaternion.LookRotation(
					(currentLink.transform.position - currentLink2.transform.position).normalized,
					dominant.up 
				);
			}
		}
	}
}
