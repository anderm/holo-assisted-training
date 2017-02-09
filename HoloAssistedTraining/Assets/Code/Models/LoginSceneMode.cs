using Assets.Code.Audio;
using Assets.Code.Managers;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Models
{
    public class LoginSceneMode : SceneMode
    {
        public LoginSceneMode()
        {
            this.SceneModeType = SceneModeType.Login;
        }

        public override bool CheckAndAdvanceScene(out bool finished)
        {
            switch (this.StepNumber)
            {
                case 0:
                    {
                        if(string.IsNullOrEmpty(MicrophoneManager.Instance.LastUserCommand))
                        {
                            TextToSpeechManager.Instance.SpeakText("Could not understand your name. Please state your name again.");
                            Communicator.Instance.StartRecording();
                        }
                        else
                        {
                            this.StepNumber++;

                            SceneManager.Instance.UserName = MicrophoneManager.Instance.LastUserCommand;
                            TextToSpeechManager.Instance.SpeakText("I understood " + SceneManager.Instance.UserName + " . Is that correct?");
                            Communicator.Instance.StartRecording();
                            finished = false;
                            return true;
                        }

                        break;
                    }
                case 1:
                    {
                        if (MicrophoneManager.Instance.LastUserCommand == "yes")
                        {
                            if (LoginManager.Instance.TryRetrieveUser(SceneManager.Instance.UserName)) {
                                var user = LoginManager.Instance.CurrentUser;
                                TextToSpeechManager.Instance.SpeakText("Welcome back " + user.first);
                                finished = true;
                                return true; 
                            }

                            TextToSpeechManager.Instance.SpeakText("Let's create a profile for you.");
                            // TODO: Handle async events: wait until profiles are loaded, then if no record found create one, then proceed once inserted...

                            finished = false;
                            return true;
                        }
                        else if (MicrophoneManager.Instance.LastUserCommand == "no")
                        {
                            TextToSpeechManager.Instance.SpeakText("Let's try again. Please state your name.");
                            Communicator.Instance.StartRecording();
                            this.StepNumber--;
                            finished = false;
                            return false;
                        }
                        else 
                        {
                            var text = MicrophoneManager.Instance.LastUserCommand;

                            TextToSpeechManager.Instance.SpeakText("Could not understand. Is your name " + SceneManager.Instance.UserName + "? Please say YES or NO.");
                            Communicator.Instance.StartRecording();
                        }

                        break;
                    }
            }

            finished = false;
            return false;
        }

        public override GameObject GetCurrentInteractiveObject()
        {
            return null;
        }

        public override void InitScene()
        {
            TextToSpeechManager.Instance.SpeakText("Welcome to Holo Assisted Training. My name is Steve and I'm your supervisor. Please say register user to begin.");

            this.StepNumber = 0;
        }
    }
}
