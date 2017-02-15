using Assets.Code.Audio;
using Assets.Code.Managers;
using HoloToolkit.Unity;
using System;
using System.Collections;
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

        public override IEnumerator CheckAndAdvanceScene(Action<bool> callback = null)
        {
            switch (this.StepNumber)
            {
                case 0:
                    {
                        if (string.IsNullOrEmpty(MicrophoneManager.Instance.LastUserCommand))
                        {
                            TextToSpeechManager.Instance.SpeakText("Could not understand your name. Please state your first and last name again.");
                            Communicator.Instance.StartRecording();
                        }
                        else
                        {
                            string[] name = MicrophoneManager.Instance.LastUserCommand.Split(' ');

                            if (name.Length == 2)
                            {
                                var firstName = name[0];
                                var lastName = name[1];
                                var fullName = MicrophoneManager.Instance.LastUserCommand;

                                this.StepNumber++;

                                SceneManager.Instance.FirstName = firstName;
                                SceneManager.Instance.LastName = lastName;
                                TextToSpeechManager.Instance.SpeakText("I understood " + fullName + " . Is that correct?");
                                Communicator.Instance.StartRecording();
                                yield return true;
                            }
                            else
                            {
                                TextToSpeechManager.Instance.SpeakText("We need your first and last name. Please say your name again like, John Doe");
                                Communicator.Instance.StartRecording();
                                yield return true;
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        var firstName = SceneManager.Instance.FirstName;
                        var lastName = SceneManager.Instance.LastName;

                        if (MicrophoneManager.Instance.LastUserCommand == "yes")
                        {
                            if (LoginManager.Instance.TryRetrieveUser(firstName, lastName))
                            {
                                var user = LoginManager.Instance.CurrentUser;
                                TextToSpeechManager.Instance.SpeakText("Welcome back " + user.first);
                                callback(true);
                                yield return true;
                            }
                            else
                            {
                                TextToSpeechManager.Instance.SpeakText("Let's create a profile for you.");
                                LoginManager.Instance.SaveUserAsync(firstName, lastName, (ok, user) =>
                                {
                                    if (ok)
                                    {
                                        TextToSpeechManager.Instance.SpeakText("All good. We created a new profile for you.");
                                        callback(true);
                                    }
                                    else
                                    {
                                        TextToSpeechManager.Instance.SpeakText("Well this is embrassing, just can't seem to save your user profile right now... Please try again.");
                                        this.StepNumber--;
                                    }
                                });
                                
                                yield return true;
                            }
                        }
                        else if (MicrophoneManager.Instance.LastUserCommand == "no")
                        {
                            TextToSpeechManager.Instance.SpeakText("Let's try again. Please state your name.");
                            Communicator.Instance.StartRecording();
                            this.StepNumber--;
                            yield return true;
                        }
                        else
                        {
                            var text = MicrophoneManager.Instance.LastUserCommand;

                            TextToSpeechManager.Instance.SpeakText("Could not understand. Is your name " + firstName + ' ' + lastName + "? Please say YES or NO.");
                            Communicator.Instance.StartRecording();
                        }

                        break;
                    }
            }
            
            yield return false;
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
