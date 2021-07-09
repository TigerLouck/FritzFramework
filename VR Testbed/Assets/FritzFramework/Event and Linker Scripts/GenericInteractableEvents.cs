using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace FritzFramework
{
	public class GenericInteractableEvents
	{
		[Serializable]
		public class InteractableLinkerConnected : UnityEvent<SteamVR_Input_Sources, InteractabilityLinker, bool> { }

		[Serializable]
		public class InteractableBool : UnityEvent<SteamVR_Input_Sources, bool> { }

		[Serializable]
		public class InteractableInt : UnityEvent<SteamVR_Input_Sources, int> { }

		[Serializable]
		public class InteractableFloat : UnityEvent<SteamVR_Input_Sources, float> { }

		[Serializable]
		public class InteractableVector2 : UnityEvent<SteamVR_Input_Sources, Vector2> { }

		[Serializable]
		public class InteractableVector3 : UnityEvent<SteamVR_Input_Sources, Vector3> { }

		[Serializable]
		public class InteractableTransform : UnityEvent<SteamVR_Input_Sources, Transform> { }

	}
}
