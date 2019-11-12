using Assets.Scripts.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TownSimulation.NewsGO.MediaGO
{
    /// <summary>
    /// Handles medias choice and the media player.
    /// Creates one button for each medias attached to the news.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/Media/MediaContainer</remarks>
    public class MediaContainer : MonoBehaviour
    {
        public NewsGameObject news;
        public GameObject buttonList;
        public MediaPlayer mediaPlayer;
        private GameObject buttonPrefab;

        private bool buttonsIsRefresh = false;

        private void Awake()
        {
            buttonPrefab = (GameObject)Resources.Load("Prefabs/News/Media/Button", typeof(GameObject));

            if (news == null)
            {
                try
                {
                    news = transform.parent.parent.GetComponent<NewsGameObject>();
                }
                catch
                {
                    Debug.Log("NewsGameObject not found");
                }
            }

            // Create all buttons associate with media
            if (news != null && news.newsInfos != null)
            {
                short nbMedia = 0;
                foreach (Media m in news.newsInfos.GetMedia())
                {
                    nbMedia++;
                    // Get components
                    GameObject button = Instantiate(buttonPrefab, buttonList.transform);
                    RectTransform buttonRect = button.GetComponent<RectTransform>();
                    ClickableUIVR buttonClickable = button.GetComponentInChildren<ClickableUIVR>();

                    // Set button text
                    button.GetComponentInChildren<Text>().text = nbMedia.ToString() + " - " + m.GetMediaTypeToString();

                    // Use to set the size of the VR clickable area
                    StartCoroutine(UpdateVRAreaButton(buttonClickable, buttonRect));

                    // Set called function when click on button
                    buttonClickable.OnClickEvent.AddListener(() => { ChangeMedia(m); });
                    button.GetComponentInChildren<Button>().onClick.AddListener(() => { ChangeMedia(m); });
                }
                if (news.newsInfos.GetMedia().Count > 0)
                {
                    ChangeMedia(news.newsInfos.GetMedia()[0]);
                }
            }
        }

        public IEnumerator UpdateVRAreaButton(ClickableUIVR buttonClickable, RectTransform buttonRect)
        {
            yield return null;
            buttonClickable.gameObject.transform.localScale = new Vector3(buttonRect.rect.width / 10, buttonClickable.gameObject.transform.localScale.y, buttonClickable.gameObject.transform.localScale.z);
        }

        public void ChangeMedia(Media m)
        {
            mediaPlayer.Stop();
            // Set to default media player display
            foreach (Transform child in mediaPlayer.transform)
            {
                child.gameObject.SetActive(false);
            }
            switch (m.GetMediaType())
            {
                case 0: // Image
                    StartCoroutine(mediaPlayer.SetImageFromWeb(m.GetUrl()));
                    break;

                case 1: // Video
                    StartCoroutine(mediaPlayer.SetVideoFromUrl(m.GetUrl()));
                    break;

                case 2: // Audio
                    StartCoroutine(mediaPlayer.SetAudioFromWeb(m.GetUrl()));
                    break;

                default:
                    mediaPlayer.errorText.text = "Unknown media type";
                    mediaPlayer.errorText.gameObject.SetActive(true);
                    break;
            }
        }

        private void Update()
        {
            if (!buttonsIsRefresh)
            {
                buttonList.SetActive(false);
                buttonList.SetActive(true);
                buttonsIsRefresh = true;
            }
        }
    }
}