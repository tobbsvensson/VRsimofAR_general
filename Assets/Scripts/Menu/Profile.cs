using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Core;
using System;
using System.Linq;
using Assets.Scripts.TownSimulation.NewsGO.CommentGO;

namespace Assets.Scripts.Menu
{
    /// <summary>
    /// Script of the profile interface
    /// </summary>
    /// <remarks>Attach to : Scenes/Login/View</remarks>
    public class Profile : MonoBehaviour
    {
        public Text nameField;
        public Text viewsField;
        public Text commentField;
        public Text savePrompt;

        public Dropdown cmtPositionDD;
        public Dropdown cmtNumbersDD;

        //tags list
        public GameObject tagTemplate;
        public GameObject content;
        public ColorPicker picker;
        private List<GameObject> tagList = new List<GameObject>();
        private Color choice;



        // Start is called before the first frame update
        void Start()
        {
            AskStatData();
            AskCommentNumberData();
            AskCommentPositionData();
            Database.GetTagColors();
            DisplayTagsList();

            picker.onValueChanged.AddListener(color =>
            {
                choice = color;
            });
        }


        // Update is called once per frame
        void Update()
        {


            if (Input.GetKeyDown("escape"))
            {
                SceneManager.LoadScene(0);
            }
        }


        /// <summary>
        /// Init. the position dropdown.
        /// </summary>
        void PopulatePositionList()
        {
            List<string> positions = Enum.GetNames(typeof(CommentGameObject.Positions)).ToList();
            cmtPositionDD.AddOptions(positions);
        }

        /// <summary>
        /// Init. the number dropdown.
        /// </summary>
        void PopulateDisplayNumberList()
        {
            List<string> display = new List<string>() { "1", "2", "3", "4", "5" };
            cmtNumbersDD.AddOptions(display);
        }


        /// <summary>
        /// Get the comment number preference save in db and set the value of the number dropdown.
        /// </summary>
        void AskCommentNumberData()
        {
            PopulateDisplayNumberList();
            cmtNumbersDD.value = cmtNumbersDD.options.FindIndex((x) => { return x.text == Database.SqlCmd("cmtNbShown"); });
        }


        /// <summary>
        /// Get the comment position preference save in db and set the value of the position dropdown.
        /// </summary>
        void AskCommentPositionData()
        {
            PopulatePositionList();
            int.TryParse(Database.SqlCmd("cmtPositionPref"), out int res);
            cmtPositionDD.value = res;
        }

        /// <summary>
        /// Get and display the statistic bind to the player.
        /// </summary>
        void AskStatData()
        {
            nameField.text += StaticClass.CurrentPlayerName;
            viewsField.text += Database.SqlCmd("nbOfView");
            commentField.text += Database.SqlCmd("nbOfComment");
        }

        /// <summary>
        /// Generate the tag list. 
        /// For each tags in the db, create a button and add it in the scrollview.
        /// If the player already save color preferences, get it and set the button color. Otherwise, set the default color.
        /// If a tag is pressed, activate the color selector. Press again the button to set the button color.
        /// </summary>
        public void DisplayTagsList()
        {
            foreach (string s in Database.GetTags())
            {
                var copy = Instantiate(tagTemplate);
                copy.transform.parent = content.transform;
                copy.transform.GetComponentInChildren<Text>().text = s;

                Color c = StaticClass.tagPrefColorList[s];


                //color the button with the pref color save
                ColorBlock cb = copy.GetComponent<Button>().colors;
                cb.normalColor = c;
                cb.selectedColor = c;
                copy.GetComponent<Button>().colors = cb;

                copy.SetActive(true);
                tagList.Add(copy);


                copy.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        if (picker.gameObject.activeSelf)
                        {
                            cb = copy.GetComponent<Button>().colors;
                            cb.normalColor = choice;
                            cb.selectedColor = choice;
                            copy.GetComponent<Button>().colors = cb;

                            picker.gameObject.SetActive(false);
                        }
                        else
                        {
                            picker.gameObject.SetActive(true);
                        }
                    }
               );
            }
        }



        /// <summary>
        /// Call when the save button is pressed.
        /// For each button in the scroll view, get its color and its text (tag name). Update those tag/color pair on the db.
        /// Get both dropdown selection and update the player preferences on the db.
        /// </summary>
        public void SaveButtonAction()
        {
            savePrompt.text = "";  //clear the prompt
            bool isColorSaved = true;
            foreach (GameObject ob in tagList)
            {
                string text = ob.GetComponentInChildren<Text>().text;
                Color color = ob.GetComponent<Button>().colors.normalColor;
                if (!Database.ChangeTagColorChoice(text, "#" + ColorUtility.ToHtmlStringRGB(color)))
                {
                    isColorSaved = false;
                    Debug.Log(text);
                    break;  // if one time the color save process failed, stop the loop
                }
            }

            if (isColorSaved && Database.PrefSucessfullySaved(Convert.ToInt32(cmtNumbersDD.options[cmtNumbersDD.value].text), cmtPositionDD.value))
            {


                savePrompt.color = Color.green;
                savePrompt.text = "Sucessfuly saved !";
            }
            else
            {
                savePrompt.color = Color.red;
                savePrompt.text = "Something wrong append ...";
                Debug.Log(isColorSaved);
            }
        }


        /// <summary>
        /// Call when the back button is pressed.
        /// Load the main menu scene.
        /// </summary>
        public void GoBackToMenu()
        {
            StaticClass.GoBackToMenu();
        }

    }
}