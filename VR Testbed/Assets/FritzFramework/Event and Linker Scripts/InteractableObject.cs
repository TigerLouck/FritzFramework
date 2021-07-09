using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace FritzFramework
{
	public class InteractableObject : MonoBehaviour
	{
		//This variable is governed by the interactability linker, during the linking and unlinking process
		[HideInInspector]
		public InteractabilityLinker currentLink = null;

		protected virtual void Start()
		{
			linkerLocks = new Dictionary<InteractabilityLinker, bool>();
		}

		//These methods are a COM-adjacent way of locking an interactable's links to multiple linkers
		public void LockLink(InteractabilityLinker link)
		{
			if (!linkerLocks.ContainsKey(link))
			{
				linkerLocks.Add(link, false);
				Debug.Log("locked " + name + " to " + link.transform.parent.name);
			}
			else
			{
				Debug.LogError("Duplicate attempt to lock an interactable!");
			}
		}

		public void UnlockLink(InteractabilityLinker link)
		{
			link.Unlock(this);
			Debug.Assert(linkerLocks.Remove(link)); // This assert might be bad, but it's sooooooo nice
			Debug.Log("unlocked " + name + " from " + link.transform.parent.name);
		}

		/// <summary>
		/// Use to check if a linker is locked to this interactable
		/// </summary>
		/// <param name="link">the linker to check locked state on</param>
		public bool IsLocked(InteractabilityLinker link)
		{
			return linkerLocks.ContainsKey(link);
		}

		public bool IsDisjointed (InteractabilityLinker link)
		{
			return linkerLocks[link];
		}

		public void SetDisjointed (InteractabilityLinker link, bool isDisjoint)
		{
			if (linkerLocks.ContainsKey(link))
			{
				// Who the fuck decided to make accessing a dictionary create an entry if none exists
				// What are you fucking doing, every other collection type throws in that situation
				linkerLocks[link] = isDisjoint;
			}
		}

		public int LockCount
		{
			get { return linkerLocks.Count; }
		}

		private Dictionary<InteractabilityLinker, bool> linkerLocks;

		//Establishing connections from spatial contact
		public GenericInteractableEvents.InteractableLinkerConnected OnLinkChanged;

	}
}
