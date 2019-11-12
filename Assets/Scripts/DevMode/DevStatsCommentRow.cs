using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Handle displaying "comment stats" of one row.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefab/DevMode/CommentRow</remarks>
    public class DevStatsCommentRow : MonoBehaviour
    {
        public Text playerName;
        public Text newsTitle;
        public Text date;
        public Text content;
        public Button deleteBttn;

        public void Fill(uint idComment, string playerName, string newsTitle, string content, DateTime date)
        {
            this.playerName.text = playerName;
            this.newsTitle.text = newsTitle;
            this.content.text = content;
            this.date.text = date.ToString("MM/dd/yyyy' - 'hh:mm tt");
            deleteBttn.onClick.AddListener(() => 
            {
                Database.DeleteComment(idComment);
                Destroy(gameObject);
            });
        }
    }
}