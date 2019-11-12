using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation.NewsGO.Reaction
{
    /// <summary>
    /// Handles the number of angry on the reaction box. 
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/ReactionCross/AngryNbr/Canvas/Text</remarks>
    public class AngryNbrView : MonoBehaviour
    {

        private void OnEnable()
        {
            this.GetComponent<Text>().text = Database.NumOfReatcionToNews("Angry", StaticClass.CurrentNewsId);
        }
    }
}
