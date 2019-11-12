using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Assets.Scripts.DevMode
{
    /// <summary>
    /// Handle the mouse buttons clicks in devmode.
    /// If you click on the town it will call DevModeNewsCreationPannel script to create a new news.
    /// </summary>
    /// <remarks>Attached to : Scenes/DevMode/NewsPlacementManager</remarks>
    public class DevModeCreateNews : MonoBehaviour
    {

        public Camera cam;
        public GameObject NewsCreationPanel;

        [HideInInspector]
        public bool newsBeingCreated;

        [HideInInspector]
        public Vector3 newsPos;


        // Use this for initialization
        void Start()
        {
            newsBeingCreated = false;
        }

        // Update is called once per frame
        void Update()
        {

            // To go back to the menu
            if (Input.GetKeyDown("escape"))
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }

            // Move the camera to see the different views of the database
            /*if (Input.GetMouseButtonDown(1))
            {
                cam.transform.position += new Vector3(700.0f, 0.0f, 0.0f);
                if (cam.transform.position.x > 2100.0f)
                {
                    cam.transform.position = new Vector3(0.0f, 300.0f, 0.0f);
                }
            }*/

            // Enter when click on the left button of the mouse
            if (Input.GetMouseButtonDown(0) && !newsBeingCreated)
            {
                // Change the pixel position in worldSpace position (the game unit)
                var mousePos = Input.mousePosition;
                // 300 because the camera is at 300 of the town in y parameter but mousePos.z is the distance of the camera, so we take the y parameter
                mousePos.z = 300;
                newsPos = cam.ScreenToWorldPoint(mousePos);

                // Omly activate the newsCreationPanel if the click is on the town
                if (newsPos.x < 150.0f && newsPos.x > -150.0f && newsPos.z < 150.0f && newsPos.z > -150.0f)
                {
                    newsBeingCreated = true;
                    NewsCreationPanel.SetActive(true);
                }
            }
        }



    }
}