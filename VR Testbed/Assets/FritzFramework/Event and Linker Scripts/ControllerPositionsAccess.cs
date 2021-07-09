using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class ControllerPositionsAccess : MonoBehaviour
    {
        public static ControllerPositionsAccess StaticAccess;

        public Transform Headset;
        public Rigidbody RightController;
        public Rigidbody LeftController;

        [HideInInspector]
        public InteractabilityLinker LHLinker;
        [HideInInspector]
        public InteractabilityLinker RHLinker;
        void Start()
        {
            StaticAccess = this;
            RHLinker = RightController.GetComponentInChildren<InteractabilityLinker>();
            LHLinker = LeftController.GetComponentInChildren<InteractabilityLinker>();
        }
    }
}