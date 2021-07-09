using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class InteractableDPad : InteractableObject
    {
        public GenericInteractableEvents.InteractableBool OnPadNorthHold;
        public GenericInteractableEvents.InteractableBool OnPadNorthDown;
        public GenericInteractableEvents.InteractableBool OnPadNorthUp;

        public GenericInteractableEvents.InteractableBool OnPadSouthHold;
        public GenericInteractableEvents.InteractableBool OnPadSouthDown;
        public GenericInteractableEvents.InteractableBool OnPadSouthUp;

        public GenericInteractableEvents.InteractableBool OnPadEastHold;
        public GenericInteractableEvents.InteractableBool OnPadEastDown;
        public GenericInteractableEvents.InteractableBool OnPadEastUp;

        public GenericInteractableEvents.InteractableBool OnPadWestHold;
        public GenericInteractableEvents.InteractableBool OnPadWestDown;
        public GenericInteractableEvents.InteractableBool OnPadWestUp;

    }
}
