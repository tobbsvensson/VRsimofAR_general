using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Core;

namespace Assets.Scripts.Menu
{
    /// <summary>
    /// Script of the login interface
    /// </summary>
    /// <remarks>Attach to : Scenes/Login/View</remarks>
    public class Login : MonoBehaviour
    {
        public InputField logNameField;
        public InputField logPasswordField;

        public Button logInButton;

        public Text logStateTxt;

        // Start is called before the first frame update
        void Start()
        {
            logInButton.onClick.AddListener(LogInButtonAction);
        }

        private void Update()
        {
            VerifyInputs();
            if (Input.GetKeyDown("escape"))
            {
                SceneManager.LoadScene(0);
            }

            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && logPasswordField.isFocused)
            {
                if (logInButton.interactable)
                    logInButton.onClick.Invoke();
            }

            if (logNameField.isFocused && Input.GetKeyDown(KeyCode.Tab))
            {
                logPasswordField.Select();
                logPasswordField.ActivateInputField();
            }

            if (logPasswordField.isFocused && Input.GetKeyDown(KeyCode.Tab))
            {
                logNameField.Select();
                logNameField.ActivateInputField();
            }
        }

        /// <summary>
        /// Set the login button interactable if the name field is not empty and password field is at least 8 long
        /// </summary>
        public void VerifyInputs()
        {
            logInButton.interactable = (logNameField.text.Length >= 1 && logPasswordField.text.Length >= 8);
        }

        /// <summary>
        /// Call when the login button is press. If the player exist in db, load main menu and set the current player
        /// </summary>
        private void LogInButtonAction()
        {
            if (Database.IsThisUserAnAuthenticPlayer(logNameField.text, logPasswordField.text))
            {
                StaticClass.CurrentPlayerName = logNameField.text;
                SceneManager.LoadScene(0);
            }
            else
            {
                logStateTxt.text = "Wrong username or password.";
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