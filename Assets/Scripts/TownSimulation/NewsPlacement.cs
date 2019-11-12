using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Core;

namespace Assets.Scripts.TownSimulation
{
    /// <summary>
    /// Handle the creation of all the news when you enter the simulation.
    /// It also deals with the escape action to return to the menu.
    /// </summary>
    /// <remarks>Attached to : Scenes/TownSimulation/EveryNews</remarks>
    public class NewsPlacement : MonoBehaviour
    {
        /// <summary>
        /// Boolean used to see if a news is open in the scene, with this you can't open two news at the same time.
        /// </summary>
        public bool aNewsIsOpen;

        public GameObject newsPreview; // Use by PreviewAreaTrigger to get the NewsPreview gameobject in the scene.

        // Use this for initialization
        void Start()
        {

            // At first, no news is open and we have to pick up a sphere to go in one
            aNewsIsOpen = true;

            foreach (News news in StaticClass.newsList)
            {
                news.GenerateNewsGameObject(transform);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("escape"))
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
        }
    }
}
