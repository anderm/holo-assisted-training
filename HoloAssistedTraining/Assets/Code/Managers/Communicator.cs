using Assets.Code.Audio;
using Assets.Code.Tools;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Managers
{
    public class Communicator : Singleton<Communicator>
    {
        [Tooltip("The waveform animation to be played while the microphone is recording.")]
        public Transform Waveform;

        [Tooltip("A text area for the recognizer to display the recognized strings.")]
        public Text DictationDisplay;

        [Tooltip("A text area to display information for the user on what to do next.")]
        public Text InstructionDisplay;

        private float origLocalScale;
        private bool animateWaveform;

        public enum Message
        {
            PressMic,
            PressStop,
            SendMessage
        };

        void Start()
        {
            origLocalScale = Waveform.localScale.y;
            animateWaveform = false;
        }

        public void StartRecording()
        {
            animateWaveform = true;
            MicrophoneManager.Instance.StartListening();
        }

        void Update()
        {
            if (animateWaveform)
            {
                Vector3 newScale = Waveform.localScale;
                newScale.y = Mathf.Sin(Time.time * 2.0f) * origLocalScale;
                Waveform.localScale = newScale;
            }
        }
    }
}
