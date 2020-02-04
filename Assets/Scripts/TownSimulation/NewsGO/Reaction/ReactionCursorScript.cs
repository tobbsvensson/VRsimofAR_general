using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Core;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;
using Valve.VR;

namespace Assets.Scripts.TownSimulation.NewsGO.Reaction
{
    /// <summary>
    /// This handles the white ball in the reaction box and it's placement on the box.
    /// Depending of it's position when you leave the news, it will add one to reaction wanted.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/ReactionCross/Cursor</remarks>
    [RequireComponent(typeof(Interactable))]
    public class ReactionCursorScript : MonoBehaviour
    {
        [EnumFlags] [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags =
            Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

        [EnumFlags]
        [Tooltip(
            "Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
        public GrabTypes grabTypes = GrabTypes.Grip;

        [Tooltip("Action to interact with UI.")]
        public SteamVR_Action_Boolean UI_Interaction_Action =
            SteamVR_Input.GetBooleanActionFromPath("/actions/default/in/InteractUI");

        [Tooltip("When detaching the object, should it return to its original parent?")]
        public bool restoreOriginalParent = false;

        [Tooltip("How far from the center that the ball should snap")]
        public float snapDistance = 0.25f;
        
        [Tooltip("Distance to the center where the user can leave the ball to make it go back to the middle")]
        public float centerTolerance = 0.05f;
        
        private bool attached = false;

        public GameObject Highlight;
        
        private Vector3 scale;
        private Quaternion rotation;

        private void Start()
        {
            var reaction = Database.ReadReactionSelected();
            var quadrant = GetQuadrantFromReaction(reaction);
            transform.localPosition = QuadrantToPosition(quadrant);
            rotation = gameObject.transform.rotation;
            scale = gameObject.transform.localScale;
        }

        private void OnDisable()
        {
            var pos = transform.localPosition;
            var quadrant = PositionToQuadrant(pos);
            var reaction = QuadrantToReaction(quadrant);

            Database.CreateOrChangeReactionToNews(reaction);
        }

        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            Highlight.SetActive(true);

            // "Catch" the throwable by holding down the interaction button instead of pressing it.
            // Only do this if it isn't attached to another hand
            if (!attached)
            {
                if (UI_Interaction_Action.GetStateDown(hand.handType))
                {
                    hand.AttachObject(gameObject, grabTypes, attachmentFlags);
                }
            }
        }


        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            Highlight.SetActive(false);

            ControllerButtonHints.HideButtonHint(hand, UI_Interaction_Action);
        }


        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            //Trigger got pressed
            if (!attached && UI_Interaction_Action.GetStateDown(hand.handType))
            {
                hand.AttachObject(gameObject, grabTypes, attachmentFlags);
                ControllerButtonHints.HideButtonHint(hand, UI_Interaction_Action);
            }
        }

        //-------------------------------------------------
        private void OnAttachedToHand(Hand hand)
        {
            attached = true;

            hand.HoverLock(null);

        }


        //-------------------------------------------------
        private void OnDetachedFromHand(Hand hand)
        {
            attached = false;

            hand.HoverUnlock(null);

            var snapPos = new Vector3(-0.1f, 0, 0);
            if (!IsWithinCenterTolerance())
            {
                var currentPos = transform.localPosition;
                var quadrant = PositionToQuadrant(currentPos);
                snapPos = QuadrantToPosition(quadrant);
            }
            
            transform.localPosition = snapPos;

            // For the deformation bug but doesn't really do the job.
            gameObject.transform.localScale = scale;
            gameObject.transform.rotation = rotation;
        }

        private bool IsWithinCenterTolerance()
        {
            var currentPos = transform.localPosition;
            var toleranceSquared = centerTolerance * centerTolerance;
            var distanceFromCenterSquared = currentPos.y * currentPos.y + currentPos.z * currentPos.z;
            return distanceFromCenterSquared < toleranceSquared;
        }


        //-------------------------------------------------
        private void HandAttachedUpdate(Hand hand)
        {
            //Trigger got released
            if (UI_Interaction_Action.GetStateUp(hand.handType))
            {
                // Detach ourselves late in the frame.
                // This is so that any vehicles the player is attached to
                // have a chance to finish updating themselves.
                // If we detach now, our position could be behind what it
                // will be at the end of the frame, and the object may appear
                // to teleport behind the hand when the player releases it.
                StartCoroutine(LateDetach(hand));
            }
        }


        //-------------------------------------------------
        private IEnumerator LateDetach(Hand hand)
        {
            yield return new WaitForEndOfFrame();

            hand.DetachObject(gameObject, restoreOriginalParent);
        }


        //-------------------------------------------------
        private void OnHandFocusAcquired(Hand hand)
        {
            gameObject.SetActive(true);
        }


        //-------------------------------------------------
        private void OnHandFocusLost(Hand hand)
        {
            gameObject.SetActive(false);
        }

        private enum ReactionQuadrant
        {
            Center,
            UpperLeft, UpperRight,
            LowerLeft, LowerRight
        }
        
        private static ReactionQuadrant GetQuadrantFromReaction(Database.Reaction reaction)
        {
            switch (reaction)
            {
                case Database.Reaction.None:
                    return ReactionQuadrant.Center;
                case Database.Reaction.Happy:
                    return ReactionQuadrant.UpperRight;
                case Database.Reaction.Sad:
                    return ReactionQuadrant.LowerRight;
                case Database.Reaction.Angry:
                    return ReactionQuadrant.LowerLeft;
                case Database.Reaction.Surprised:
                    return ReactionQuadrant.UpperLeft;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reaction), reaction, "Invalid reaction type.");
            }
        }

        private static Database.Reaction QuadrantToReaction(ReactionQuadrant quadrant)
        {
            switch (quadrant)
            {
                case ReactionQuadrant.Center:
                    return Database.Reaction.None;
                case ReactionQuadrant.UpperLeft:
                    return Database.Reaction.Surprised;
                case ReactionQuadrant.UpperRight:
                    return Database.Reaction.Happy;
                case ReactionQuadrant.LowerLeft:
                    return Database.Reaction.Angry;
                case ReactionQuadrant.LowerRight:
                    return Database.Reaction.Sad;
                default:
                    throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, "Invalid quadrant type.");
            }
        }

        private static bool IsQuadrantUpper(ReactionQuadrant quadrant)
        {
            return quadrant == ReactionQuadrant.UpperLeft || quadrant == ReactionQuadrant.UpperRight;
        }
        
        private static bool IsQuadrantRight(ReactionQuadrant quadrant)
        {
            return quadrant == ReactionQuadrant.UpperRight || quadrant == ReactionQuadrant.LowerRight;
        }
        
        private Vector3 QuadrantToPosition(ReactionQuadrant quadrant)
        {
            if (quadrant == ReactionQuadrant.Center)
            {
                return new Vector3(-0.1f, 0, 0);
            }
            
            return new Vector3(0.0f,
                IsQuadrantUpper(quadrant) ? snapDistance : -snapDistance,
                IsQuadrantRight(quadrant) ? -snapDistance : snapDistance);
        }

        private ReactionQuadrant PositionToQuadrant(Vector3 position)
        {
            var isUpper = position.y >  float.Epsilon;
            var isLower = position.y < -float.Epsilon;
            var isRight = position.z < -float.Epsilon;
            var isLeft  = position.z >  float.Epsilon;

            var quadrant = ReactionQuadrant.Center;
            if (isUpper && isLeft)  quadrant = ReactionQuadrant.UpperLeft;
            if (isUpper && isRight) quadrant = ReactionQuadrant.UpperRight;
            if (isLower && isLeft)  quadrant = ReactionQuadrant.LowerLeft;
            if (isLower && isRight) quadrant = ReactionQuadrant.LowerRight;

            return quadrant;
        }
    }
}
