using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class SimpleGrab : MonoBehaviour
	{
		protected Rigidbody rb;
		protected bool grabbed = false;
		protected SteamVR_Input_Sources grabber = SteamVR_Input_Sources.Any; //"any" source is default to indicate no grabber
		protected InteractabilityLinker currentLink = null;
		protected Vector3 heldObjectLastPosition = new Vector3();
		protected InteractableObject controllingInteractable;

		protected virtual void Start()
		{
			rb = GetComponent<Rigidbody>();
			controllingInteractable = GetComponent<InteractableObject>();
		}

		public virtual void ToggleGrab(SteamVR_Input_Sources source, bool value)
		{
			grabbed = !grabbed;
			rb.isKinematic = grabbed;
			grabber = source;
			if (!rb.isKinematic)
				rb.velocity = (transform.position - heldObjectLastPosition) * Time.deltaTime; //this doesn't work. too bad!
			Debug.Log("Grabbed:" + true);
		}

		public virtual void InteractableLinkChanged(SteamVR_Input_Sources source, InteractabilityLinker hand, bool isConnecting)
		{
			if (isConnecting)
			{
				currentLink = hand;
				//Debug.Log("linked with" + source);
			}
			else if (hand == currentLink && !grabbed) //If the currently connected hand is not what's calling to disconnect, don't
			{
				currentLink = null; 
				//Debug.Log("unlinked with" + source);
			}
		}

		// Update is called once per frame
		protected virtual void Update()
		{
			if (currentLink != null)
			{
				Debug.DrawRay(transform.position, InputUtils.GetTransformFromInSource(currentLink.thisSource).position - transform.position, Color.black,.016f,false);
			}
			if (grabbed)
			{
				Transform grabTrans = InputUtils.GetTransformFromInSource(grabber);
				if (grabTrans == null) return;
				heldObjectLastPosition = transform.position;
				transform.position = grabTrans.position;
				transform.rotation = grabTrans.rotation;

			}
		}
	}
}