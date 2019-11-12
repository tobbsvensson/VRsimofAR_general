using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;
using Assets.Scripts.Core;
using System.Linq;
using Assets.Scripts.TownSimulation.NewsGO.CommentGO;

namespace Assets.Scripts.Menu
{
    /// <summary>
    /// Script of the main menu interface
    /// </summary>
    /// <remarks>Attach to : Scenes/Login/View</remarks>
    public class MainMenu : MonoBehaviour
    {

        public Button playGameButton;
        public Button profilButton;
        public Button devMode;

        public Text sortedByTxt;
        public Text newsPrompt;
        public Text state;

        // Buttons from notif list
        public Image notifDateImage;
        public Image notifClosestImage;
        public Image notifPopularityImage;
        public Dropdown notifTagsDropdown;
        private Color notifSelectedColor = new Color(124f / 255f, 162f / 255f, 142f / 255f);
        private Color notifNotSelectedColor = new Color(177f / 255f, 232f / 255f, 202f / 255f);

        //notification list
        public GameObject notifTemplate;
        public GameObject content;

        public const int MAX_NOTIF_TO_DISPLAY = 20;




        public List<GameObject> listBtnNews = new List<GameObject>();


        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void Start()
        {
            Database.GetTagColors();
            if (StaticClass.CurrentPlayerName != "")
            {
                state.text = "User log :  " + StaticClass.CurrentPlayerName;
                profilButton.interactable = (true);
                playGameButton.interactable = (true);
                devMode.interactable = (true);
                StaticClass.newsList.Clear();
                Database.GenerateNewsList();
                List<string> tmp = Database.GetTags();
                tmp.Sort();
                notifTagsDropdown.AddOptions(tmp);
                NotifSortedByDate();
            }
            else
            {
                profilButton.interactable = (false);
                playGameButton.interactable = (false);
                devMode.interactable = (false);
            }
        }

        /*******************************/
        /****** Notification List ******/
        /*******************************/



        /// <summary>
        /// Generate the notification list. 
        /// For each news in the db, create a button and add it in the scrollview.
        /// If the player already save color preferences, get it and set the button color.
        /// If a notification button is pressed, add it to the activation beacon list (StaticClass.newsBeaconedList).
        /// </summary>
        public void DisplayNews(List<News> ln)
        {

            foreach (News n in ln)
            {
                var copy = Instantiate(notifTemplate);
                copy.transform.parent = content.transform;
                copy.transform.GetComponentInChildren<Text>().text = n.GetTitle() + "\n" + n.GetTagsToString() + " - " + n.GetDist() + "m - (" + n.nbOfView + "-" + n.nbComment + ").";
                copy.SetActive(true);

                if (StaticClass.newsBeaconedList.Exists(x => x == n.GetId()))
                {
                    ColorBlock cb = copy.GetComponent<Button>().colors;
                    cb.normalColor = n.GetNewsColor();
                    copy.GetComponent<Button>().colors = cb;
                }




                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        if (StaticClass.newsBeaconedList.Exists(x => x == n.GetId()))
                        {
                            StaticClass.newsBeaconedList.Remove(n.GetId());
                            copy.GetComponent<Button>().colors = notifTemplate.GetComponent<Button>().colors;

                        }
                        else
                        {
                            StaticClass.newsBeaconedList.Add(n.GetId());

                            ColorBlock cb = copy.GetComponent<Button>().colors;
                            cb.normalColor = n.GetNewsColor();
                            copy.GetComponent<Button>().colors = cb;
                        }

                    //TODO ugly way to refresh the content
                    content.SetActive(false);
                        content.SetActive(true);
                    }
                );

                listBtnNews.Add(copy);
            }
        }

        /**************************/
        /****** MENU BUTTONS ******/
        /**************************/

        /// <summary>
        /// Load register scene
        /// </summary>
        public void GoToRegister()
        {
            SceneManager.LoadScene(3);
        }

        /// <summary>
        /// Load login scene
        /// </summary>
        public void GoToLogin()
        {
            SceneManager.LoadScene(4);
        }

        /// <summary>
        /// Load setting scene
        /// </summary>
        public void GoToSetting()
        {
            SceneManager.LoadScene(5);
        }

        /// <summary>
        /// Load devmode scene
        /// </summary>
        public void GoToDevMode()
        {
            SceneManager.LoadScene(6);
        }

        /// <summary>
        /// Load the simulation scene
        /// </summary>
        public void Play()
        {
            StaticClass.nbrCommentDisplayed = Database.ReadNbrCommentDisplayed();
            StaticClass.CommentPosition = (CommentGameObject.Positions)Database.ReadCommentPosition();
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Should load the VR device to go from dev mode to town simulation but doesn't seem to work
        /// Once you have gone to devmode you need to quit the application to not cause bug when going to Town simulation.
        /// </summary>
        IEnumerator LoadDevice(string newDevice, bool enable)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = enable;
        }

        void EnableVR()
        {
            StartCoroutine(LoadDevice("OpenVR", true));
        }

        void DisableVR()
        {
            StartCoroutine(LoadDevice("", false));
        }


        /**************************************/
        /****** NOTIFICATION LIST ACTION ******/
        /**************************************/

        /// <summary>
        /// Call when the date button is pressed.
        /// Clear the content of the scrollview (notification list).
        /// Then display the news order by desc. date (done in the sql request)
        /// <see cref="Database"/> , line 91
        /// </summary>
        public void NotifSortedByDate()
        {
            ClearNotification();
            DisplayNews(StaticClass.newsList);
            sortedByTxt.text = "The 20 recent news";
            notifTagsDropdown.value = 0; // Reset tags dropdown to default
            notifDateImage.color = notifSelectedColor;
        }

        /// <summary>
        /// Call when the distance button is pressed.
        /// Clear the content of the scrollview (notification list).
        /// Then display the news order by desc. distance (euclidian)
        /// </summary>
        public void NotifSortedByDist()
        {
            ClearNotification();
            List<News> SortedList = StaticClass.newsList.OrderBy(o => o.GetDist()).ToList();
            DisplayNews(SortedList);
            sortedByTxt.text = "The 20 closest news";
            notifTagsDropdown.value = 0; // Reset tags dropdown to default
            notifClosestImage.color = notifSelectedColor;
        }

        /// <summary>
        /// Call when the distance button is pressed.
        /// Clear the content of the scrollview (notification list).
        /// Then display the news order by asc. distance (euclidian)
        /// </summary>
        public void NotifSortedByPoularity()
        {
            ClearNotification();
            List<News> SortedList = StaticClass.newsList.OrderByDescending(o => o.nbOfView).ToList();
            DisplayNews(SortedList);
            sortedByTxt.text = "The 20 most-viewed news";
            notifTagsDropdown.value = 0; // Reset tags dropdown to default
            notifPopularityImage.color = notifSelectedColor;
        }

        /// <summary>
        /// Call when the distance button is pressed.
        /// Clear the content of the scrollview (notification list).
        /// Then display the news order by selected tag 
        /// </summary>
        public void NotifSortedByTag()
        {
            if (notifTagsDropdown.value != 0) // First option of tags dropdown ("Tag") do nothing when chosen
            {
                ClearNotification();
                List<News> SortedList = new List<News>();
                string tag = notifTagsDropdown.options[notifTagsDropdown.value].text;
                foreach (News n in StaticClass.newsList)
                {
                    if (n.GetTags().Contains(tag))
                        SortedList.Add(n);
                }
                DisplayNews(SortedList);
                sortedByTxt.text = "The 20 oldest " + tag + " news";
                notifTagsDropdown.GetComponent<Image>().color = notifSelectedColor;
            }
        }

        /// <summary>
        /// Clear the content of the scrollview by destroying all the button.
        /// </summary>
        public void ClearNotification()
        {
            if (listBtnNews.Count > 0)
            {
                foreach (GameObject go in listBtnNews)
                {
                    Destroy(go);
                }
                listBtnNews.Clear();
            }

            //set to default the color of all the sort button
            notifDateImage.color = notifNotSelectedColor;
            notifClosestImage.color = notifNotSelectedColor;
            notifPopularityImage.color = notifNotSelectedColor;
            notifTagsDropdown.GetComponent<Image>().color = notifNotSelectedColor;
        }
    }
}