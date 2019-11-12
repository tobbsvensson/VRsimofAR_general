using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;

namespace Assets.Scripts.Core
{
    /// <summary>
    /// Attach it to a gameobject to make it clickable.
    /// The <see cref="OnClickEvent"/> is called when <see cref="Boolean_Action"/> SteamVR action is performed hover the gameobject.
    /// </summary>
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Collider))]
    public class ClickableUIVR : MonoBehaviour
    {
        /// <summary>
        /// The VR action from SteamVR inputs to perform to call OnClickEvent
        /// </summary>
        public SteamVR_Action_Boolean Boolean_Action = SteamVR_Input.GetBooleanAction("InteractUI");
        /// <summary>
        /// The event call
        /// </summary>
        public UnityEvent OnClickEvent;

        // Start is called before the first frame update
        void Start()
        {

            if (OnClickEvent == null)
            {
                OnClickEvent = new UnityEvent();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void HandHoverUpdate(Hand hand)
        {
            if (Boolean_Action.GetStateDown(hand.handType))
            {
                OnClickEvent.Invoke();
            }
        }
    }
}