using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation.NewsGO.Reaction
{
    /// <summary>
    /// Handles the number of happy on the reaction box. 
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/ReactionCross/HappyNbr/Canvas/Text</remarks>
    public class HappyNbrView : MonoBehaviour
    {

        private void OnEnable()
        {
            this.GetComponent<Text>().text = Database.NumOfReatcionToNews("Happy", StaticClass.CurrentNewsId);
        }
    }
}
