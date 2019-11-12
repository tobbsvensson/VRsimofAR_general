using UnityEngine;
using Assets.Scripts.Core;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.TownSimulation.NewsGO.CommentGO
{
    /// <summary>
    /// Handles reject button behavior.
    /// Deletes <see cref="CommentGameObject"/> gameobject if is pressed and don't add the comment on the database.
    /// </summary>
    /// <remarks>Attach to : Resources/Prefabs/Comment/Buttons/RejectButton/VRTrigger</remarks>
    /// <seealso cref="Assets.Scripts.Core.ClickableUIVR" />
    [RequireComponent(typeof(Interactable))]
    public class NewsCommentReject : ClickableUIVR
    {

        public GameObject Comment;

        // Action when you click the delete button, delete all the comment
        public void RejectAction()
        {
            Destroy(Comment);
            // Wait 1 second to delete the reject button to not cause bug
            Destroy(gameObject, 1);
        }
    }
}
