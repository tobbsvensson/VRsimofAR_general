using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation.NewsGO.Reaction
{
    /// <summary>
    /// Handles the number of sad on the reaction box. 
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/ReactionCross/SadNbr/Canvas/Text</remarks>
    public class SadNbrView : MonoBehaviour
    {

        private void OnEnable()
        {
            GetComponent<Text>().text = Database.NumOfReactionToNews(Database.Reaction.Sad, StaticClass.CurrentNewsId);
        }
    }
}