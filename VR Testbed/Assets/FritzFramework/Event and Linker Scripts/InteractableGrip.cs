using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class InteractableGrip : InteractableObject
    {
        public GenericInteractableEvents.InteractableBool OnGripHold;
        public GenericInteractableEvents.InteractableBool OnGripDown;
        public GenericInteractableEvents.InteractableBool OnGripUp;
    }
}