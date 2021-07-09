using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class InteractableTrigger : InteractableObject
    {
        public GenericInteractableEvents.InteractableBool OnTriggerClick;
        public GenericInteractableEvents.InteractableBool OnTriggerClickDown;
        public GenericInteractableEvents.InteractableBool OnTriggerClickUp;
        public GenericInteractableEvents.InteractableFloat OnTriggerPositionChanged;
    }
}