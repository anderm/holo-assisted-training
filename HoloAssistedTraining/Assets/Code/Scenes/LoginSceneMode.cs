using Assets.Code.Audio;
using Assets.Code.Managers;
using Assets.Code.Models;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Scenes
{
    public class LoginSceneMode : SceneMode
    {
        public LoginSceneMode(Action<bool> sceneFinishedCallback) : base(sceneFinishedCallback)
        {
            this.SceneModeType = SceneModeType.Login;
        }

        private void LoadUsersCompleted(bool success, UserProfile[] profiles)
        {
            if(!success)
            {
                TextToSpeechManager.Instance.SpeakText("Something went wrong getting the user profiles. Where's the wifi gone when you need it?");
            }
        }

        private void RetrieveUserCompleted(bool succes, UserProfile user)
        {
            if (succes)
            {
                TextToSpeechManager.Instance.SpeakText("Welcome back " + user.first);
                this.SceneFinishedCallback(true);
            }
            else
            {
                var firstName = user.first;
                var lastName = user.last;
                TextToSpeechManager.Instance.SpeakText("Let's create a profile for you.");
                LoginManager.Instance.SaveUserAsync(firstName, lastName, (ok, savedUser) =>
                {
                    if (ok)
                    {
                        TextToSpeechManager.Instance.SpeakText("All good. We created a new profile for you.");
                        this.SceneFinishedCallback(true);
                    }
                    else
                    {
                        TextToSpeechManager.Instance.SpeakText("Well this is embrassing, just can't seem to save your user profile right now... Please try again.");
                        this.StepNumber--;
                    }
                });
            }
        }

        public override IEnumerator CheckAndAdvanceScene()
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

                                LoginManager.Instance.FirstName = firstName;
                                LoginManager.Instance.LastName = lastName;
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
                        var firstName = LoginManager.Instance.FirstName;
                        var lastName = LoginManager.Instance.LastName;

                        if (MicrophoneManager.Instance.LastUserCommand == "yes")
                        {
                            LoginManager.Instance.TryFindUserAsync(firstName, lastName, RetrieveUserCompleted);
                            yield return true;
                            
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
            LoginManager.Instance.LoadUsersAsync(LoadUsersCompleted);
            TextToSpeechManager.Instance.SpeakText("Welcome to Holo Assisted Training. My name is Steve and I'm your supervisor. Please say register user to begin.");

            this.StepNumber = 0;
        }
    }
}
