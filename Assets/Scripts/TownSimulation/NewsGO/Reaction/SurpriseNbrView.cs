using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation.NewsGO.Reaction
{
    /// <summary>
    /// Handles the number of surprise on the reaction box. 
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/ReactionCross/SurpriseNbr/Canvas/Text</remarks>
    public class SurpriseNbrView : MonoBehaviour
    {

        private void OnEnable()
        {
            this.GetComponent<Text>().text = Database.NumOfReactionToNews("Surprised", StaticClass.CurrentNewsId);
        }
    }
}