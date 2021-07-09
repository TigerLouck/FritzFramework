using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FritzFramework
{
    public class InteractableMenuButton : InteractableObject
    {
        public GenericInteractableEvents.InteractableBool OnMenuHold;
        public GenericInteractableEvents.InteractableBool OnMenuClickDown;
        public GenericInteractableEvents.InteractableBool OnMenuClickUp;
    }
}
