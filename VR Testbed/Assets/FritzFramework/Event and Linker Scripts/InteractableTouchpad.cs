using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class InteractableTouchpad : InteractableObject
    {
        public GenericInteractableEvents.InteractableVector2 OnTouchpadTouchCoord;
        public GenericInteractableEvents.InteractableBool OnPadClickHold;
        public GenericInteractableEvents.InteractableBool OnPadClickDown;
        public GenericInteractableEvents.InteractableBool OnPadClickUp;
        public GenericInteractableEvents.InteractableBool OnPadTouchHold;
        public GenericInteractableEvents.InteractableBool OnPadTouchDown;
        public GenericInteractableEvents.InteractableBool OnPadTouchUp;

    }
}
