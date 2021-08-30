using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class InteractabilityLinker : MonoBehaviour
	{
		public SteamVR_Input_Sources thisSource;

		public Rigidbody RB;
		List<InteractableObject> currentInteractables = new List<InteractableObject>();

		private void Start()
		{
			RB = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// Establishes connection with interactables
		/// </summary>
		/// <param name="other">Struck collider</param>
		private void OnTriggerEnter(Collider other)
		{
			// GetInParent allows for a hierarchichal interactable placement system, as this will get all interactables to the root.
			InteractableObject[] interactables = other.GetComponentsInParent<InteractableObject>();
			foreach (InteractableObject interactable in interactables)
			{
				bool alreadyPresent = currentInteractables.Contains(interactable);
				if (interactable != null && !alreadyPresent)
				{
					interactable.currentLink = this;
					interactable.OnLinkChanged.Invoke(thisSource, this, true);
					currentInteractables.Add(interactable);
					interactable.SetDisjointed(this, false);
				}
				else if (alreadyPresent)
				{
					interactable.SetDisjointed(this, false);
				}
			}
		}

		/// <summary>
		/// Cuts connection with interactables
		/// </summary>
		/// <param name="other">Collider the trigger is leaving</param>
		private void OnTriggerExit(Collider other)
		{

			InteractableObject interactable = other.GetComponentInParent<InteractableObject>();
			if (interactable != null)
			{
				// interactables may be capable of separating from the linker, but wish to retain the connection for whatever reason
				// Check if the interactable is locked to this linker
				if (!interactable.IsLocked(this))
				{
					Debug.DrawRay(transform.position, interactable.transform.position - transform.position, Color.cyan, 40, false);
					Disconnect(interactable);
				}
				else
				{
					Debug.DrawRay(transform.position, interactable.transform.position - transform.position, Color.cyan, 1, false);
					// We are connected but physically separated
					interactable.SetDisjointed(this, true);
				}
			}
		}

		/// <summary>
		/// Because objects can unlock with disjointed hitboxes, trigger exit may fail to fire- 
		/// -because it already has done so. So this is called to ensure that the disconnection happens
		/// </summary>
		/// <param name="unlocker">the interactable calling this</param>
		public void Unlock(InteractableObject unlocker)
		{
			// Unlock happens before removal from the lock list
			if (unlocker.IsDisjointed(this) && unlocker.IsLocked(this))
			{
				Debug.DrawRay(transform.position, unlocker.transform.position - transform.position, Color.cyan, 40, false);
				Disconnect(unlocker);
			}
		}

		private void Disconnect(InteractableObject interactable)
		{
			Debug.DrawRay(transform.position, interactable.transform.position - transform.position, Color.cyan, 40, false);
			interactable.currentLink = null;
			interactable.OnLinkChanged.Invoke(thisSource, this, false);
			currentInteractables.Remove(interactable);// Praying to god the get hash works
		}

		// Events are transmitted in a more-or-less brute force manner: 
		// Negligibly faster, modestly harder to maintain, and a flippant violation of DRY
		// Probably could have done this with generics


		//Trigger
		public void TriggerClickChanged(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("trigger click invoked");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.black, 30, false);
				(currentInteractables[i] as InteractableTrigger)?.OnTriggerClick.Invoke(source, value);
			}
		}

		public void TriggerClickDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("trigger click down invoked");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTrigger)?.OnTriggerClickDown.Invoke(source, value);
			}
		}

		public void TriggerClickUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("trigger click up invoked");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTrigger)?.OnTriggerClickUp.Invoke(source, value);
			}
		}

		public void TriggerPositionChanged(SteamVR_Behaviour_Single behaviour, SteamVR_Input_Sources source, float value, float value2)
		{
			//Debug.Log("trigger position changed invoked to " + value);
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTrigger)?.OnTriggerPositionChanged.Invoke(source, value);
			}
		}

		// Grip
		public void GripHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("Grip Held");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.black, 30, false);
				(currentInteractables[i] as InteractableGrip)?.OnGripHold.Invoke(source, value);
			}
		}

		public void GripDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("Grip Down");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableGrip)?.OnGripDown.Invoke(source, value);
			}
		}

		public void GripUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("Grip up");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableGrip)?.OnGripUp.Invoke(source, value);
			}
		}

		// Touchpad
		public void TouchpadTouchCoord(SteamVR_Behaviour_Vector2 behaviour, SteamVR_Input_Sources source, Vector2 value, Vector2 value2)
		{
			//Debug.Log("touchpad touching at " + value);
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnTouchpadTouchCoord.Invoke(source, value);
			}
		}

		public void TouchpadClickHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			Debug.Log("touchpad clicked");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.black, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadClickHold.Invoke(source, value);
			}
		}

		public void TouchpadClickDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("touchpad click down");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadClickDown.Invoke(source, value);
			}
		}

		public void TouchpadClickUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("Touchpad click up");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadClickUp.Invoke(source, value);
			}
		}

		public void TouchpadTouchHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("Touchpad Held");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadTouchHold.Invoke(source, value);
			}
		}

		public void TouchpadTouchDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("touchpad touch down");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadTouchDown.Invoke(source, value);
			}
		}

		public void TouchpadTouchUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			//Debug.Log("touchpad touch up");
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableTouchpad)?.OnPadTouchUp.Invoke(source, value);
			}
		}

		// TODO: Touchpad d-pad
		//Touchpad North
		public void TouchpadNorthHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadNorthHold.Invoke(source, value);
			}
		}

		public void TouchpadNorthDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadNorthDown.Invoke(source, value);
			}
		}

		public void TouchpadNorthUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadNorthUp.Invoke(source, value);
			}
		}

		//Touchpad South
		public void TouchpadSouthHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadSouthHold.Invoke(source, value);
			}
		}

		public void TouchpadSouthDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadSouthDown.Invoke(source, value);
			}
		}

		public void TouchpadSouthUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadSouthUp.Invoke(source, value);
			}
		}

		//Touchpad East
		public void TouchpadEastHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadEastHold.Invoke(source, value);
			}
		}

		public void TouchpadEastDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadEastDown.Invoke(source, value);
			}
		}

		public void TouchpadEastUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadEastUp.Invoke(source, value);
			}
		}

		//Touchpad West
		public void TouchpadWestHold(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadWestHold.Invoke(source, value);
			}
		}

		public void TouchpadWestDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadWestDown.Invoke(source, value);
			}
		}

		public void TouchpadWestUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			for (int i = currentInteractables.Count - 1; i >= 0; i--)
			{
				Debug.DrawRay(transform.position, currentInteractables[i].transform.position - transform.position, Color.red, 30, false);
				(currentInteractables[i] as InteractableDPad)?.OnPadWestUp.Invoke(source, value);
			}
		}
	}
}
