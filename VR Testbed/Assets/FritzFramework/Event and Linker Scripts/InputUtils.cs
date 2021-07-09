using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace FritzFramework
{
	public class InputUtils : MonoBehaviour
	{

		public static Transform GetTransformFromInSource(SteamVR_Input_Sources source)
		{
			if (source == SteamVR_Input_Sources.RightHand)
			{
				return ControllerPositionsAccess.StaticAccess.RightController.transform;
			}
			if (source == SteamVR_Input_Sources.LeftHand)
			{
				return ControllerPositionsAccess.StaticAccess.LeftController.transform;
			}

			Debug.LogError("Failed to retrieve transform from input source. Is your event source correctly configured?");
			return null;
		}

		public static Rigidbody GetRigidbodyFromInSource(SteamVR_Input_Sources source)
		{
			if (source == SteamVR_Input_Sources.RightHand)
			{
				return ControllerPositionsAccess.StaticAccess.RightController;
			}
			if (source == SteamVR_Input_Sources.LeftHand)
			{
				return ControllerPositionsAccess.StaticAccess.LeftController;
			}

			Debug.LogError("Failed to retrieve rigidbody from input source. Is your event source correctly configured?");
			return null;
		}

		public static InteractabilityLinker GetLinkerFromInSource (SteamVR_Input_Sources source)
		{
			if (source == SteamVR_Input_Sources.RightHand)
			{
				return ControllerPositionsAccess.StaticAccess.RHLinker;
			}
			if (source == SteamVR_Input_Sources.LeftHand)
			{
				return ControllerPositionsAccess.StaticAccess.LHLinker;
			}

			Debug.LogError("Failed to retrieve linker from input source. Is your event source correctly configured?");
			return null;
		}
	}
}