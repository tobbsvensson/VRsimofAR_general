using Assets.Scripts.TownSimulation.NewsGO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TownSimulation.Preview
{
    /// <summary>
    /// Handles the news preview, the content of the preview and his positioning depending on the player position.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/NewsPreview</remarks>
    public class NewsPreview : MonoBehaviour
    {
        private NewsGameObject news = null;

        private Transform followHead;
        public Vector3 panelPreviewPostion = new Vector3(-0.3f, 0f, 0.7f);

        public TagListGameObject tagList;
        public ViewNbrComment viewNbrComment;
        public ViewNbrView viewNbrView;

        private bool refresh = false;


        // Use to get infos from the news pointed on and display its
        public void SetPreviewInfos(NewsGameObject news)
        {
            this.news = news;

            tagList.newsGameObject = news;
            tagList.gameObject.SetActive(true);

            viewNbrView.GetComponent<TextMesh>().text = news.newsInfos.nbOfView.ToString();
            viewNbrComment.GetComponent<TextMesh>().text = news.newsInfos.nbComment.ToString();

        }

        // Start is called before the first frame update
        void Awake()
        {
            followHead = GameObject.Find("FollowHead").transform;
            tagList = GetComponentInChildren<TagListGameObject>();
            viewNbrComment = GetComponentInChildren<ViewNbrComment>();
            viewNbrView = GetComponentInChildren<ViewNbrView>();
        }

        private void OnEnable()
        {
            if (news != null)
            {
                transform.Find("Panel/Title").GetComponent<Text>().text = news.TitleInNews.text;
                transform.Find("Panel/Infos").gameObject.GetComponent<Text>().text = news.content.text;
                transform.position = followHead.transform.TransformPoint(panelPreviewPostion);
                transform.rotation = Quaternion.LookRotation(transform.position - followHead.transform.position, Vector3.up);
            }
        }

        private void OnDisable()
        {
            foreach (Transform tag in tagList.gameObject.transform)
            {
                Destroy(tag.gameObject);
            }
            refresh = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!refresh && tagList.transform.childCount > 0)
            {
                tagList.transform.GetChild(0).gameObject.SetActive(false);
                tagList.transform.GetChild(0).gameObject.SetActive(true);
                refresh = true;
            }
        }
    }
}