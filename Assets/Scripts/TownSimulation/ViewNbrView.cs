using UnityEngine;
using System;
using Assets.Scripts.Core;
using Assets.Scripts.TownSimulation.NewsGO;

namespace Assets.Scripts.TownSimulation
{
    /// <summary>
    /// This handles the display of the number of views of the new
    /// </summary>
    /// <remarks>Attached to :
    /// Resources/Prefabs/News/News/NewsSphere/NewsTitle/ViewsNumber
    /// Resources/Prefabs/NewsPreview/Panel/HGrid/View/ViewsNumber</remarks>
    public class ViewNbrView : MonoBehaviour
    {
        public NewsGameObject news;

        public void ReadViewNbr()
        {
            string nb = Database.ReadViewNum(news.Id);
            this.GetComponent<TextMesh>().text = nb;
            // Keep coherence with news object
            news.newsInfos.nbOfView = Convert.ToUInt32(nb);
        }
    }
}