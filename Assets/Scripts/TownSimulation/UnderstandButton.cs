using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.TownSimulation
{
    /// <summary>
    /// Handle the Understand button at the beginning.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/IndicationBeginning/.../UnderstandButton</remarks>
    [RequireComponent(typeof(Interactable))]
    public class UnderstandButton : MonoBehaviour
    {
        private GameObject Teleport; // Reference to teleport gameobject
        public GameObject IndicationBegin; // Reference to Beginning panel
        public GameObject EveryNews; // Reference to EveryNews gameobject

        private void Start()
        {
            if (Teleport == null)
                Teleport = GameObject.Find("TeleportController");
            if (IndicationBegin == null)
                IndicationBegin = GameObject.Find("IndicationBeginning");
        }

        /// <summary>
        /// Called when you click the Understand button. Delete the Understand panel and button.
        /// </summary>
        public void UnderstandAction()
        {
            var tpcontrol = Teleport.GetComponent<TeleportController>();
            tpcontrol.ChangeTeleport(true);

            EveryNews.GetComponent<NewsPlacement>().aNewsIsOpen = false;

            Destroy(IndicationBegin);
            // Wait 1 second to delete the IndicationBegin button to not cause bug
            Destroy(gameObject, 1);
        }
    }
}
