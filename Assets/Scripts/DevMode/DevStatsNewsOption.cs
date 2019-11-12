using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// TagMedia scene manager.
    /// Links tags and medias to a news.
    /// </summary>
    /// <remarks>Attached to : Scenes/TagMedia/View</remarks>
    public class DevStatsNewsOption : MonoBehaviour
    {
        public GameObject tagTemplate;
        public GameObject content;
        public List<string> topics;

        public Text tagsSelected;


        public Dropdown ddMedia;
        public InputField url;
        public Button addMediaBtn;
        public Dictionary<string, int> mediaList = new Dictionary<string, int>();
        public GameObject mediaTemplate;
        public GameObject mediaContent;
        public List<GameObject> listBtnMedia;

        public Text prompt;

        public InputField Title;
        public InputField TextNews;
        public GameObject NewsPlacementManager;

        // Start is called before the first frame update
        void OnEnable()
        {
            DisplayTagsList();
        }

        // Update is called once per frame
        void Update()
        {
            VerifyInput();
            ddMedia.onValueChanged.AddListener(delegate
            {
                url.text = "";
            });

        }

        private void VerifyInput()
        {
            // > 4 min. due to the file's type (png, ogg, mpa, ...) and different of none
            addMediaBtn.interactable = (url.text.Length > 4 && ddMedia.value > 0);
            url.interactable = (ddMedia.value != 0);
        }

        public void DisplayTagsList()
        {
            foreach (string s in Database.GetTags())
            {
                var copy = Instantiate(tagTemplate);
                copy.transform.parent = content.transform;
                copy.transform.GetComponentInChildren<Text>().text = s;
                copy.SetActive(true);

                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        if (topics.Exists(x => x == s))
                        {
                            topics.Remove(s);

                            ColorBlock cb = copy.GetComponent<Button>().colors;
                            cb.normalColor = Color.white;
                            copy.GetComponent<Button>().colors = cb;
                        }
                        else
                        {
                            if (topics.Count < 4)
                            {
                                topics.Add(s);

                                ColorBlock cb = copy.GetComponent<Button>().colors;
                                cb.normalColor = Color.green;
                                copy.GetComponent<Button>().colors = cb;
                            }

                        }

                    //TODO ugly way to refresh the content
                    content.SetActive(false);
                        content.SetActive(true);

                        tagsSelected.text = TagsToString();


                    }
               );
            }
        }

        public string TagsToString()
        {
            string buff = "";
            foreach (string s in topics)
            {
                buff += " / " + s;
            }
            return buff;
        }

        public void AddMediaAction()
        {
            if (!mediaList.ContainsKey(url.text) && mediaList.Count < 4)
            {
                mediaList.Add(url.text, ddMedia.value);
                RefreshMediaList();
            }

        }

        private void DisplayMediaList()
        {
            int index = 1;
            foreach (KeyValuePair<string, int> entry in mediaList)
            {
                string medType = "";
                switch (entry.Value)
                {
                    case 1:
                        medType = "Image";
                        break;

                    case 2:
                        medType = "Video";
                        break;

                    case 3:
                        medType = "Audio";
                        break;
                }

                var copy = Instantiate(mediaTemplate);
                copy.transform.parent = mediaContent.transform;
                copy.transform.GetComponentInChildren<Text>().text = index + " - " + medType + "                            X  ";  /// Sorry
                listBtnMedia.Add(copy);
                copy.SetActive(true);

                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        mediaList.Remove(entry.Key);
                        Destroy(copy);
                    }
               );


                //TODO ugly way to refresh the content
                mediaContent.SetActive(false);
                mediaContent.SetActive(true);

                index++;
            }
        }

        public void RefreshMediaList()
        {
            if (listBtnMedia.Count > 0)
            {
                foreach (GameObject go in listBtnMedia)
                {
                    Destroy(go);
                }
                listBtnMedia.Clear();
            }
            DisplayMediaList();
        }

        public bool SaveData()
        {
            Database.CreateANews(Title.text.ToString(), TextNews.text.ToString(), NewsPlacementManager.GetComponent<DevModeCreateNews>().newsPos.x, NewsPlacementManager.GetComponent<DevModeCreateNews>().newsPos.z);
            if (Database.LastNewsCreated() != -1)
            {
                foreach (KeyValuePair<string, int> entry in mediaList)
                {
                    if (!Database.InsertMedia(Database.LastNewsCreated(), entry.Key, entry.Value))
                    {
                        Debug.Log("error insert media");
                        return false;
                    }
                }

                foreach (string s in topics)
                {
                    if (!Database.InsertTopic(Database.LastNewsCreated(), s))
                    {
                        Debug.Log("error insert topic");
                        return false;
                    }
                }

                return true;
            }
            Debug.Log("error insert news");
            return false;
        }

        public void SaveBtnAction()
        {
            if (SaveData())
            {
                prompt.text = " News Successfully created !";
                prompt.color = Color.green;

            }
            else
            {
                prompt.text = "Something wrong happened ...";
                prompt.color = Color.red;
            }
        }
    }
}