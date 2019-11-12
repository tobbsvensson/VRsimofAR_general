using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Handle displaying "last view stats" of one row.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefab/DevMode/Row</remarks>
    public class DevStatsRow : MonoBehaviour
    {
        public Text playerName;
        public Text newsTitle;
        public Text reaction;
        public Text nbCmt;
        public Text date;

        public void Fill(string playerName, string newsTitle, string reaction, uint nbCmt, DateTime date)
        {
            this.playerName.text = playerName;
            this.newsTitle.text = newsTitle;
            this.reaction.text = reaction;
            this.nbCmt.text = nbCmt.ToString();
            this.date.text = date.ToString("MM/dd/yyyy' - 'hh:mm tt");
        }
    }
}