using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class SimpleAmbiTeleport : MonoBehaviour
	{
		// Controller members
		public Transform lController;
		public Transform rController;

		//arc velocities
		public float velocity = 1;
		public float timeStep = .5f;
		public float yFloor = -5;

		private LineRenderer arcRenderer;
		private bool lineVisible;
		private Transform pressSource;
		// Start is called before the first frame update
		void Start()
		{
			arcRenderer = GetComponent<LineRenderer>();
		}

		private void Update()
		{
			// If the line is visible, we're calculating teleportation every frame
			if (lineVisible)
			{
				// TODO: everything
			}
		}

		public void OnPadDown(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{
			pressSource = InputUtils.GetTransformFromInSource(source);
			lineVisible = true;
		}

		public void OnPadUp(SteamVR_Behaviour_Boolean behaviour, SteamVR_Input_Sources source, bool value)
		{

		}
	}
}