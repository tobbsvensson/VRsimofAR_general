using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Main page of the devmode. Here you can create news and tags.
    /// </summary>
    public class AdminPage : MonoBehaviour
    {
        //tags list
        public GameObject tagTemplate;
        public GameObject tagContent;
        public GameObject tagAddPanel;
        public GameObject tagDeletePanel;

        //news list
        public GameObject newsTemplate;
        public GameObject newsContent;
        public GameObject newsDeletePanel;

        public List<GameObject> newsListGO;
        public List<GameObject> tagListGO;

        private News ToDelete;

        // Start is called before the first frame update
        void Start()
        {
            DisplayNewsList();
            DisplayTagsList();
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void AddNews()
        {
            // Disable VR to go to the dev mode since it's on a screen.
            DisableVR();
            SceneManager.LoadScene("DevMode", LoadSceneMode.Single);
        }



        IEnumerator LoadDevice(string newDevice, bool enable)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = enable;
        }

        void DisableVR()
        {
            StartCoroutine(LoadDevice("", false));
        }


        public void AddTags()
        {
            tagAddPanel.SetActive(true);
        }

        public void CancelTagActionBtn()
        {
            tagAddPanel.SetActive(false);
            tagDeletePanel.SetActive(false);
            newsDeletePanel.SetActive(false);
        }

        /// <summary>
        /// Call when you want to submit a new tag
        /// </summary>
        public void SaveAddTag()
        {
            string newTag = tagAddPanel.GetComponentInChildren<InputField>().text;
            Debug.Log(newTag);
            if (newTag != "")
            {
                if (IsTagAlreadyExist(newTag))
                {
                    tagAddPanel.GetComponentInChildren<Text>().text = "This tag already exists";
                }
                else
                {
                    if (Database.InsertTag(newTag))
                    {
                        tagAddPanel.GetComponentInChildren<Text>().text = newTag + " added successfully";
                        tagAddPanel.GetComponentInChildren<InputField>().text = "";    ///clear the input field
                        DisplayTagsList();  ///refresh tag list
                        Database.AddDefaultNotificationByTag(newTag);
                    }
                    else
                    {
                        tagAddPanel.GetComponentInChildren<Text>().text = "Something wrong happened";
                    }
                }
            }
            else
            {
                tagAddPanel.GetComponentInChildren<Text>().text = "This field can't be empty";
            }
        }


        /// <summary>
        /// Generate the tag list. 
        /// For each tags in the db, create a button and add it in the scrollview.
        /// If a tag is pressed, activate the delete panel.
        /// </summary>
        public void DisplayTagsList()
        {
            ClearList(tagListGO);
            foreach (string s in Database.GetTags())
            {
                var copy = Instantiate(tagTemplate);
                copy.transform.parent = tagContent.transform;
                copy.transform.GetComponentInChildren<Text>().text = "    " + s;
                copy.SetActive(true);
                tagListGO.Add(copy);

                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        tagDeletePanel.SetActive(true);
                        tagDeletePanel.GetComponentInChildren<Text>().text = s;
                    }
               );
            }
        }


        /// <summary>
        /// Generate the news list. 
        /// For each news in the db, create a button and add it in the scrollview.
        /// If a news is pressed, activate the delete panel.
        /// </summary>
        public void DisplayNewsList()
        {
            ClearList(newsListGO);
            foreach (News n in StaticClass.newsList)
            {
                var copy = Instantiate(newsTemplate);
                copy.transform.parent = newsContent.transform;
                copy.transform.GetComponentInChildren<Text>().text = "    " + n.GetId() + " - " + n.GetTitle();
                copy.SetActive(true);
                newsListGO.Add(copy);

                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        newsDeletePanel.SetActive(true);
                        newsDeletePanel.GetComponentInChildren<Text>().text = n.GetId() + " - " + n.GetTitle();
                        ToDelete = n;
                    }
               );
            }
        }

        /// <summary>
        /// Call when you want to delete a tag
        /// </summary>
        /// <param name="t"></param>
        public void DeleteTag(Text t)
        {
            Database.RemoveTag(t.text);
            DisplayTagsList();
            tagDeletePanel.SetActive(false);
        }

        /// <summary>
        /// Call when you want to delete a news
        /// </summary>
        public void DeleteNews()
        {
            Database.DeleteNews(ToDelete.GetId());
            StaticClass.newsList.Remove(ToDelete);
            DisplayNewsList();  //refresh
            newsDeletePanel.SetActive(false);
            ToDelete = null;
        }

        /// <summary>
        /// Call when you want to add a tag.
        /// </summary>
        /// <param name="s">tag name</param>
        /// <returns>True if tag already exist</returns>
        public bool IsTagAlreadyExist(string s)
        {
            foreach (GameObject go in tagListGO)
            {
                if (go.GetComponentInChildren<Text>().text == "    " + s)
                {
                    return true;
                }
            }
            return false;
        }


        public void ClearList(List<GameObject> lgo)
        {
            if (lgo.Count > 0)
            {
                foreach (GameObject go in lgo)
                {
                    Destroy(go);
                }
                lgo.Clear();
            }
        }

        public void GoBackToMenu()
        {
            StaticClass.GoBackToMenu();
        }

        public void GoToStatistics()
        {
            SceneManager.LoadScene(8);
        }

    }
}