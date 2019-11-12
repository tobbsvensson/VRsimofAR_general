using UnityEngine;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation
{

    /// <summary>
    /// Handle the placement of the panel with the indication at the beginning.
    /// You can take it and move it around too.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/IndicationBeginning</remarks>
    /// <seealso cref="Assets.Scripts.Core.Grabbable" />
    public class BeginInformation : Grabbable
    {

        // Reference to player head.
        private GameObject HeadCollider;

        private void Start()
        {
            HeadCollider = GameObject.Find("HeadCollider");

            // Put the news in front of the player the first time the player pick up the newsSphere
            transform.position = new Vector3(HeadCollider.transform.position.x, 1.5f, HeadCollider.transform.position.z);
            transform.rotation = new Quaternion(0, HeadCollider.transform.rotation.y, 0, HeadCollider.transform.rotation.w);
            transform.Translate(new Vector3(HeadCollider.transform.forward.x, 0.0f, HeadCollider.transform.forward.z), Space.World);
        }


        private void OnEnable()
        {

            HeadCollider = GameObject.Find("HeadCollider");

            // Put the news in front of the player every time the player pick up the newsSphere
            transform.position = new Vector3(HeadCollider.transform.position.x, 1.5f, HeadCollider.transform.position.z);
            transform.rotation = new Quaternion(0, HeadCollider.transform.rotation.y, 0, HeadCollider.transform.rotation.w);
            transform.Translate(new Vector3(HeadCollider.transform.forward.x, 0.0f, HeadCollider.transform.forward.z), Space.World);

        }
    }
}
