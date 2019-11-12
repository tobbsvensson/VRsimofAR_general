using Assets.Scripts.Core;
using Assets.Scripts.DevMode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Handles the DevStats scene : buttons behavior, generate and display statistic from database.
    /// </summary>
    /// <remarks>Attached to : Scenes/DevStats/View</remarks>
    public class DevStats : MonoBehaviour
    {

        public Dropdown playersDD;
        public Dropdown newsDD;
        public GameObject contentParent;
        public Text title;
        public Text description;

        public Button statsButton;
        public Button commentsButton;

        public GameObject headerStats;
        public GameObject headerComments;

        private Dictionary<int, uint> players = new Dictionary<int, uint>(); // Index in playersDD, player ID
        private Dictionary<int, uint> news = new Dictionary<int, uint>(); // Index in newsDD, news ID

        private GameObject viewRowPrefab;
        private List<DevStatsData> viewToDisplay;
        private GameObject commentsRowPrefab;
        private Dictionary<Comment, string> commentsToDisplay;

        private void Awake()
        {
            viewRowPrefab = (GameObject)Resources.Load("Prefabs/DevMode/Row", typeof(GameObject));
            commentsRowPrefab = (GameObject)Resources.Load("Prefabs/DevMode/CommentRow", typeof(GameObject));
        }

        // Start is called before the first frame update
        void Start()
        {
            // ********** DATA GENERATION *********** //
            Database.GenerateNewsList();
            foreach (KeyValuePair<uint, string> player in Database.GetPlayers())
            {
                playersDD.options.Add(new Dropdown.OptionData(player.Value));
                players.Add(playersDD.options.Count - 1, player.Key);
            }

            List<uint> idsNewsOrderByTitle = StaticClass.newsList.OrderBy(x => x.GetTitle()).Select(n => n.GetId()).ToList();
            List<string> titlesNewsOrderByTitle = StaticClass.newsList.Select(n => n.GetTitle()).OrderBy(x => x).ToList();

            for (int i = 0; i < idsNewsOrderByTitle.Count; i++)
            {
                newsDD.options.Add(new Dropdown.OptionData(titlesNewsOrderByTitle[i]));
                news.Add(newsDD.options.Count - 1, idsNewsOrderByTitle[i]);
            }
            // ************************************** //
            DisplayStats();
        }

        public void RefreshResult()
        {
            if (headerStats.activeSelf)
                DisplayStats();
            else if (headerComments)
                DisplayComments();
        }

        public void StatsButton()
        {
            title.text = "Statistics - Last Views";
            description.text = "Last view, reaction and number of comments from players on news - Sorted by last viewed date";
            statsButton.GetComponent<Image>().color = new Color(161 / (float)255, 255 / (float)255, 143 / (float)255);
            commentsButton.GetComponent<Image>().color = new Color(142 / (float)255, 186 / (float)255, 134 / (float)255);
            headerStats.SetActive(true);
            headerComments.SetActive(false);
            DisplayStats();
        }

        public void CommentsButton()
        {
            title.text = "Comments";
            description.text = "Comments from players on news - Sorted by date of creation";
            statsButton.GetComponent<Image>().color = new Color(142 / (float)255, 186 / (float)255, 134 / (float)255);
            commentsButton.GetComponent<Image>().color = new Color(161 / (float)255, 255 / (float)255, 143 / (float)255);
            headerStats.SetActive(false);
            headerComments.SetActive(true);
            DisplayComments();
        }

        private void DisplayStats()
        {
            
            foreach (Transform child in contentParent.transform)
            {
                if (child.GetSiblingIndex() > 1)
                    Destroy(child.gameObject);
            }

            if (playersDD.value == 0)
            {
                if (newsDD.value == 0)
                    viewToDisplay = Database.GetDevStatsView(0, 0, false, false); // Select all players and all news in SQL cmd
                else
                    viewToDisplay = Database.GetDevStatsView(news[newsDD.value], 0, true, false); // Select all players in SQL cmd
            }
            else
            {
                if (newsDD.value == 0)
                    viewToDisplay = Database.GetDevStatsView(0, players[playersDD.value], false, true); // Select all news in SQL cmd
                else
                    viewToDisplay = Database.GetDevStatsView(news[newsDD.value], players[playersDD.value], true, true); // Specific player and news
            }

            StartCoroutine(FillInStats());
        }

        private IEnumerator FillInStats()
        {
            foreach (DevStatsData data in viewToDisplay)
            {
                yield return null;
                GameObject row = Instantiate(viewRowPrefab, contentParent.transform);
                row.GetComponent<DevStatsRow>().Fill(data.playerName, data.newsTitle, data.reaction, data.nbCmt, data.date);
            }
        }

        private void DisplayComments()
        {
            
            foreach (Transform child in contentParent.transform)
            {
                if (child.GetSiblingIndex() > 1)
                    Destroy(child.gameObject);
            }

            if (playersDD.value == 0)
            {
                if (newsDD.value == 0)
                    commentsToDisplay = Database.GetDevStatsComment(0, 0, false, false); // Select all players and all news in SQL cmd
                else
                    commentsToDisplay = Database.GetDevStatsComment(news[newsDD.value], 0, true, false); // Select all players in SQL cmd
            }
            else
            {
                if (newsDD.value == 0)
                    commentsToDisplay = Database.GetDevStatsComment(0, players[playersDD.value], false, true); // Select all news in SQL cmd
                else
                    commentsToDisplay = Database.GetDevStatsComment(news[newsDD.value], players[playersDD.value], true, true); // Specific player and news
            }

            StartCoroutine(FillInComment());
        }

        private IEnumerator FillInComment()
        {
            foreach (KeyValuePair<Comment, string> e in commentsToDisplay)
            {
                yield return null;
                Comment comment = e.Key;
                string newsTitle = e.Value;
                GameObject row = Instantiate(commentsRowPrefab, contentParent.transform);
                row.GetComponent<DevStatsCommentRow>().Fill(comment.IdComment, comment.Author, newsTitle, comment.Content, comment.Date);
            }
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(6); // Go back to Admin Scene
        }
    }
}