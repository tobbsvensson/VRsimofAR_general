using UnityEngine;
using System;
using Assets.Scripts.Core;
using Assets.Scripts.TownSimulation.NewsGO;

namespace Assets.Scripts.TownSimulation
{
    /// <summary>
    /// This handles the display of the comment number of the new
    /// </summary>
    /// <remarks>Attached to : 
    /// Resources/Prefabs/News/News/NewsSphere/NewsTitle/CommentNumber
    /// Resources/Prefabs/NewsPreview/Panel/HGrid/Comment/CommentNumber</remarks>
    public class ViewNbrComment : MonoBehaviour
    {

        public NewsGameObject news;

        public void DisplayCommentNbr()
        {
            string nb = Database.ReadComntNum(news.Id);
            this.GetComponent<TextMesh>().text = nb;
            // Keep coherence with news object
            news.newsInfos.nbComment = Convert.ToUInt32(nb);
        }
    }
}