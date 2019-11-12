using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Handle the creation of a news after you choose the position by clicking on the town in devmode.
    /// </summary>
    /// <remarks>Attached to : Scenes/DevMode/NewsPlacementManager/NewsCreationPanel</remarks>
    public class DevModeNewsCreationPannel : MonoBehaviour
    {

        public Button Cancel;
        public Button Next;
        public InputField Title;
        public InputField TextNews;
        public GameObject NewsPlacementManager;
        public GameObject OptionPanel;
        public GameObject NewsContentPanel;

        // Use this for initialization
        void Start()
        {
            Cancel.onClick.AddListener(CancelAction);
            Next.onClick.AddListener(NextAction);
        }

        private void Update()
        {
            VerifyInputs();

            if (Title.isFocused && Input.GetKeyDown(KeyCode.Tab))
            {
                TextNews.Select();
                TextNews.ActivateInputField();
            }
        }

        /*private void OnDisable()
        {
            Title.text = "";
            TextNews.text = "";
        }*/

        void CancelAction()
        {
            NewsPlacementManager.GetComponent<DevModeCreateNews>().newsBeingCreated = false;
            this.gameObject.SetActive(false);
        }

        /*
        void OkAction()
        {
            // Create the news
            Debug.Log(NewsPlacementManager.GetComponent<DevModeCreateNews>().newsPos.x);
            Database.CreateANews(Title.text.ToString(), TextNews.text.ToString(), NewsPlacementManager.GetComponent<DevModeCreateNews>().newsPos.x, NewsPlacementManager.GetComponent<DevModeCreateNews>().newsPos.z);

            // Say that we are not creating a news at the moment so that we can click on the map
            NewsPlacementManager.GetComponent<DevModeCreateNews>().newsBeingCreated = false;
            this.gameObject.SetActive(false);
        }
        */

        public void NextAction()
        {
            NewsContentPanel.SetActive(false);
            OptionPanel.SetActive(true);
        }

        public void VerifyInputs()
        {
            Next.interactable = (Title.text.Length >= 1 && TextNews.text.Length >= 1);
        }

        public void Back()
        {
            NewsContentPanel.SetActive(true);
            OptionPanel.SetActive(false);
        }

        public void Menu()
        {
            SceneManager.LoadScene(6);
        }
    }
}