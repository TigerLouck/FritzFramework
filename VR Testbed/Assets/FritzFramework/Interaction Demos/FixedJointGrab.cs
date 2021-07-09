using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class FixedJointGrab : SimpleGrab
	{
		protected FixedJoint joint;

		public override void ToggleGrab(SteamVR_Input_Sources source, bool value)
		{
			//Toggle the grab, and then handle whichever state we're going into
			grabbed = !grabbed;
			if (grabbed)
			{
				controllingInteractable.LockLink(currentLink); //Impossible-to-fulfill configurations can cause the hitbox to exit while grabbed

				if (joint == null)
				{
					joint = gameObject.AddComponent<FixedJoint>();
				}
				grabber = source;
				Transform grabberTrans = InputUtils.GetTransformFromInSource(source);
				heldObjectLastPosition = grabberTrans.position;
				joint.connectedBody = grabberTrans.GetComponentInChildren<Rigidbody>();
				joint.connectedAnchor = transform.position - heldObjectLastPosition;
				joint.enablePreprocessing = false;
				Debug.Log("Grabbed:" + gameObject);
			}
			else
			{
				if (joint != null)
				{
					controllingInteractable.UnlockLink(currentLink);
					Destroy(joint);
					grabber = SteamVR_Input_Sources.Any;
				}
				else
				{
					Debug.LogError("missing joint during ungrab process! what!");
				}
				Debug.Log("UnGrabbed:" + gameObject);
			}
		}

		protected override void Update()
		{
			//Do nothing lol. architectural mistake.
		}
	}
}
