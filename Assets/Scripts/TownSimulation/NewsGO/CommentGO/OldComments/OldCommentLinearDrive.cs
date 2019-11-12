//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Drives a linear mapping based on position between 2 positions
//
//=============================================================================

// Script modified from LinearDrive (Valve), replace original LinearMapping with a float property to access position

using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.TownSimulation.NewsGO.CommentGO.OldComments
{
    /// <summary>
    /// Handles the automatic opening and closing of the old comment paper scroll.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/OldCommentPaperScroll/OpenScroll</remarks>
    [RequireComponent(typeof(Interactable))]
    public class OldCommentLinearDrive : MonoBehaviour
    {
        public GameObject scrollPaper;
        public Transform startPosition;
        public Transform endPosition;
        //public LinearMapping linearMapping;
        public bool repositionGameObject = true;
        public bool maintainMomemntum = true;
        /// <summary>
        /// This parameter handles the speed of opening and closing.
        /// </summary>
        public float momemtumDampenRate = 5.0f;

        protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

        protected float initialMappingOffset;
        protected int numMappingChangeSamples = 5;
        protected float[] mappingChangeSamples;
        protected float prevMapping = 0.0f;
        protected float mappingChangeRate;
        protected int sampleCount = 0;

        protected Interactable interactable;

        // Actual position of the scroll in percentage from startPositon to endPosition (value 0 to 1)
        private float scrollPosition = 0.0f;

        public float ScrollPosition
        {
            get
            {
                return scrollPosition;
            }

            // Desactivate the old comment scroll when close to avoid UI overlap
            set
            {
                scrollPosition = value;
                if (scrollPosition == 0.0f && scrollPaper.activeSelf)
                {
                    scrollPaper.SetActive(false); // To avoid UI overlap
                }
                else if (!scrollPaper.activeSelf)
                {
                    scrollPaper.SetActive(true);
                }
            }
        }

        protected virtual void Awake()
        {
            mappingChangeSamples = new float[numMappingChangeSamples];
            interactable = GetComponent<Interactable>();
        }

        protected virtual void Start()
        {
            /*if ( linearMapping == null )
            {
                linearMapping = GetComponent<LinearMapping>();
            }

            if ( linearMapping == null )
            {
                linearMapping = gameObject.AddComponent<LinearMapping>();
            }

            if (scrollPaper == null)
            {
                scrollPaper = transform.parent.Find("ScrollPaper").gameObject;
            }*/

            initialMappingOffset = ScrollPosition;

        }

        protected virtual void OnEnable()
        {
            ScrollPosition = 0.0f;
            scrollPaper.SetActive(false);
        }

        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                initialMappingOffset = ScrollPosition - CalculateLinearMapping(hand.transform);
                sampleCount = 0;
                mappingChangeRate = 0.0f;

                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            }
        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            UpdateLinearMapping(hand.transform);

            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            CalculateMappingChangeRate();
        }


        protected void CalculateMappingChangeRate()
        {
            //Compute the mapping change rate
            mappingChangeRate = 0.0f;
            int mappingSamplesCount = Mathf.Min(sampleCount, mappingChangeSamples.Length);
            if (mappingSamplesCount != 0)
            {
                for (int i = 0; i < mappingSamplesCount; ++i)
                {
                    mappingChangeRate += mappingChangeSamples[i];
                }
                mappingChangeRate /= mappingSamplesCount;
            }
        }

        protected void UpdateLinearMapping(Transform updateTransform)
        {
            prevMapping = ScrollPosition;
            ScrollPosition = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));

            mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = (1.0f / Time.deltaTime) * (ScrollPosition - prevMapping);
            sampleCount++;

            if (repositionGameObject)
            {
                transform.position = Vector3.Lerp(startPosition.position, endPosition.position, ScrollPosition);
            }
        }

        protected float CalculateLinearMapping(Transform updateTransform)
        {
            Vector3 direction = endPosition.position - startPosition.position;
            float length = direction.magnitude;
            direction.Normalize();

            Vector3 displacement = updateTransform.position - startPosition.position;

            return Vector3.Dot(displacement, direction) / length;
        }


        protected virtual void Update()
        {
            if (interactable.attachedToHand == null)
            {
                if (maintainMomemntum && ScrollPosition > 0.0f && ScrollPosition < 1.0f)
                {
                    // Open the scroll completely if you start an opening movement
                    if (mappingChangeRate > 0.0f)
                    {
                        ScrollPosition = Mathf.Clamp01(ScrollPosition + momemtumDampenRate * 0.01f);
                    }
                    // Close the scroll completely if you start a closing movement
                    else if (mappingChangeRate < -0.0f)
                    {
                        ScrollPosition = Mathf.Clamp01(ScrollPosition - momemtumDampenRate * 0.01f);
                    }
                    // Open the scroll completely if there is no movement and the scroll if half opened
                    else if (ScrollPosition >= 0.5f)
                    {
                        ScrollPosition = Mathf.Clamp01(ScrollPosition + momemtumDampenRate * 0.01f);
                    }
                    // Close the scroll completely if there is no movement and the scroll if half closed
                    else
                    {
                        ScrollPosition = Mathf.Clamp01(ScrollPosition - momemtumDampenRate * 0.01f);
                    }
                }

                if (repositionGameObject)
                {
                    transform.position = Vector3.Lerp(startPosition.position, endPosition.position, ScrollPosition);
                }
            }
        }
    }
}