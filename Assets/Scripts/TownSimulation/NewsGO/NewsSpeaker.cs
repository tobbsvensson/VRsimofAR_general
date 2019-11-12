using UnityEngine;
using SpeechLib;
using UnityEngine.UI;
using Assets.Scripts.Core;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.TownSimulation.NewsGO
{
    /// <summary>
    /// This handles the speaker, it uses the interop.speechlib dll.
    /// The position and replacement is the same as the microphone.
    /// It starts to read the article when in hand and stops when releases.
    /// </summary>
    /// <remarks>Attached to : Resources/Prefabs/News/News/InTheNews/Canvas_Story/Speaker</remarks>
    /// <seealso cref="Assets.Scripts.Core.Grabbable" />
    [RequireComponent(typeof(Interactable))]
    public class NewsSpeaker : Grabbable
    {

        private Vector3 transformInit;

        private SpVoice voice;

        public GameObject Text;

        // Used to see if there is a hand to release when you go out of the news and which one it is
        private Hand handToRelease;

        private void OnEnable()
        {
            this.transformInit = transform.localPosition;
        }

        private void OnDisable()
        {
            if (handToRelease != null)
            {
                OnDetachedFromHand(handToRelease);
            }

        }

        //-------------------------------------------------
        protected new void OnAttachedToHand(Hand hand)
        {
            base.OnAttachedToHand(hand);
            handToRelease = hand;

            // we initialise the voice whenever the speaker is taken, because there is no stop fonction, so we pause when realease and reinitialise when taken
            this.voice = new SpVoice();

            // We read in async so that the player cam do something else at the same time
            this.voice.Speak(Text.GetComponent<Text>().text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }


        //-------------------------------------------------
        protected new void OnDetachedFromHand(Hand hand)
        {
            base.OnDetachedFromHand(hand);
            handToRelease = null;

            // Return the speaker to no rotation (it began at 180)
            transform.localRotation = new Quaternion(0, 180, 0, 0);

            transform.localPosition = this.transformInit;

            // Pause the voice, we can continue with Resume, but we never do it so pause is like to stop for us
            this.voice.Pause();
        }
    }
}
