using UnityEngine;
using Valve.VR;
using Assets.Scripts.Core;
using Valve.VR.InteractionSystem;
using Assets.Scripts.TownSimulation.NewsGO.CommentGO;

namespace Assets.Scripts.TownSimulation.NewsGO
{
    /// <summary>
    /// Handles positioning of all the object inside the news, load the comments and handles scroll on news panel.
    /// The media container, reaction box and the microphone are children of the object attached to this script, that's why, this handles positioning of all the object in the news.
    /// </summary>
    /// <remarks>Attach to : Resources/Prefabs/News/News/InTheNews/Canvas_Story</remarks>
    /// <seealso cref="Grabbable"/>
    [RequireComponent(typeof(Interactable))]
    public class NewsPanel : Grabbable
    {
        [Tooltip("Action to scroll the article.")]
        public SteamVR_Action_Vector2 scrollArticleAction = SteamVR_Input.GetVector2ActionFromPath("/actions/default/in/Scroll");

        private Player player;
        // Use by NewsComment script to have the first position of the player when entered in the news
        private Transform playerFirstTransform;

        // Change this parameter to change the distance from the player of the news item, microphone and reaction box.
        public float panelDistance = 0.8f;
        // Change this parameter to change the heigth of the news item, microphone and reaction box.
        public float panelHeightDownOffset = 0.3f;
        
        public Transform commentParent;
        public GameObject contentNews;
        public GameObject comments;
        public GameObject oldCommentsScroll;
        public GameObject tagsGO;

        private RectTransform numberToMove;
        private float whereIsTheArticle;

        // Used to see if there is a hand to release when you go out of the news and which one it is
        private Hand handToRelease;

        private bool loadComment = false;

        public void LoadComment()
        {
            // Set first comment position
            CommentGameObject.SetFirstCommentPosition(playerFirstTransform);

            // Load all the comments from the database associate to the news
            Comment.commentsList = Database.QueryComments(StaticClass.CurrentNewsId);

            // Generate N first gameobject comments (N = user setting in StaticClass)
            CommentGameObject.GenerateComments(commentParent);

            comments.SetActive(true);

            // Display old comments scroll if there are old comments to display (depend on user setting) 
            if (StaticClass.nbrCommentDisplayed < Comment.commentsList.Count)
            {
                oldCommentsScroll.SetActive(true);
            }
        }

        private void Start()
        {
            whereIsTheArticle = 0.0f;
        }

        private void Update()
        {
            // Load comments next frame after panel is loaded
            if (loadComment)
            {
                LoadComment();
                loadComment = false;
            }
        }

        private void OnEnable()
        {
            player = FindObjectOfType<Player>();
            playerFirstTransform = player.hmdTransform;

            // Put the news in front of the player every time the player pick up the newsSphere
            Vector3 faceDirection = new Vector3(playerFirstTransform.forward.x, 0, playerFirstTransform.forward.z).normalized;
            transform.position = playerFirstTransform.position + (faceDirection * panelDistance) + (Vector3.down * panelHeightDownOffset);
            transform.rotation = Quaternion.LookRotation(faceDirection, Vector3.up);

            loadComment = true;
        }

        private void OnDisable()
        {

            if(handToRelease != null)
            {
                OnDetachedFromHand(handToRelease);
            }

            oldCommentsScroll.SetActive(false);
            comments.SetActive(false);

            // Clear game object comments and data comments
            Comment.commentsList.Clear();
            foreach (Transform cmt in comments.transform)
            {
                Destroy(cmt.gameObject);
            }
        }

        //-------------------------------------------------
        protected new void OnAttachedToHand(Hand hand)
        {
            base.OnAttachedToHand(hand);
            handToRelease = hand;

        }


        //-------------------------------------------------
        protected new void OnDetachedFromHand(Hand hand)
        {
            base.OnDetachedFromHand(hand);
            handToRelease = null;
        }


        //-------------------------------------------------
        // Function to scroll the article
        private void ScrollArticle(Hand hand)
        {

            // The component to change to make the article scroll
            RectTransform numberToMove = (RectTransform)contentNews.GetComponent(typeof(RectTransform));

            // Find how to get the position of the thumb on the touchpad
            if (scrollArticleAction != null)
            {
                if (scrollArticleAction.GetAxis(hand.handType).y < 0)
                {
                    // Move the article from the top to the bottom by step
                    whereIsTheArticle = whereIsTheArticle + 0.003f;
                    numberToMove.offsetMax = new Vector2(numberToMove.offsetMax.x, whereIsTheArticle);
                }

                if (scrollArticleAction.GetAxis(hand.handType).y > 0)
                {
                    if (whereIsTheArticle >= 0.0f)
                    {
                        // Move the article from the bottom to the top by step
                        whereIsTheArticle = whereIsTheArticle - 0.003f;
                        numberToMove.offsetMax = new Vector2(numberToMove.offsetMax.x, whereIsTheArticle);
                    }
                }
            }
        }
    }
}
