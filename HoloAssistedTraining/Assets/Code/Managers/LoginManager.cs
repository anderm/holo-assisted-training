using Assets.Code.Models;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity3dAzure.AppServices;
using UnityEngine;

namespace Assets.Code.Managers
{
    public class LoginManager : Singleton<LoginManager>
    {
        // async request delegates
        public delegate void LoadedUsers(bool success, UserProfile[] UserProfiles);
        public static event LoadedUsers OnLoadedUsers;

        public delegate void FoundUser(bool success, UserProfile userProfile);
        public static event FoundUser OnFoundUser;

        public delegate void SavedUser(bool success, UserProfile userProfile);
        public static event SavedUser OnSavedUser;

        public delegate void ChangedUser(UserProfile userProfile);
        public static event ChangedUser OnChangedUser;

        private UserProfile[] UserProfiles;
        public UserProfile CurrentUser
        {
            get {
                return CurrentUser;
            }
            set
            {
                CurrentUser = value;
                OnChangedUser(CurrentUser);
            }
        }

        public LoginManager()
        {
            LoadUsersAsync();
        }

        public void LoadUsersAsync()
        {
            StartCoroutine(AppServicesManager.Instance.User.Read<UserProfile>(LoadUsersComplete));
        }

        private void LoadUsersComplete(IRestResponse<UserProfile[]> response)
        {
            if (response.IsError)
            {
                TextToSpeechManager.Instance.SpeakText("Something went wrong getting the user profiles. Where's the wifi gone when you need it?"); // TODO: move speech into scene manager
                OnLoadedUsers(false, null);
                return;
            }
            UserProfiles = response.Data;
            OnLoadedUsers(true, UserProfiles);
        }

        public bool TryRetrieveUser(string fullName)
        {
            string[] name = fullName.Split(' ');
            if(name.Length == 2)
            {
                return TryRetrieveUser(name[0], name[1]);
            }
            Debug.LogError("Expected a first and last name.");
            return false;
        }

        public bool TryRetrieveUser(string firstName, string lastName)
        {
            if (!ValidValues(firstName, lastName)) return false;

            if (UserProfiles == null || UserProfiles.Length == 0)
            {
                return false;
            }

            foreach (UserProfile user in UserProfiles)
            {
                if(user.first.Equals(firstName, StringComparison.OrdinalIgnoreCase) && user.last.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                {
                    CurrentUser = user;
                    return true;
                }
            }

            // record not found, create new user profile
            //SaveUserAsync(firstName, lastName);

            return false;
        }

        public void TryFindUserAsync(string firstName, string lastName)
        {
            if (!ValidValues(firstName, lastName)) return;

            // Try to find from loaded profiles first (limited to 50 users by default)
            if (TryRetrieveUser(firstName, lastName))
            {
                OnFoundUser(true, CurrentUser);
                return;
            }

            string filter = string.Format("first eq '{0}' and last eq '{1}'", firstName, lastName);
            CustomQuery query = new CustomQuery(filter);
            StartCoroutine(AppServicesManager.Instance.User.Query<UserProfile>(query, TryFindUserComplete));
        }

        private void TryFindUserComplete(IRestResponse<UserProfile[]> response)
        {
            if (response.IsError)
            {
                OnFoundUser(false, null);
                return;
            }
            if (response.Data.Length == 0)
            {
                // user not found
                OnFoundUser(false, null);
                return;
            }
            if (response.Data.Length != 1)
            {
                Debug.LogWarning("Found multiple profiles, did you clone yourself?"); // TODO: handle persons which have duplicate names
            }
            CurrentUser = response.Data[0];
            OnFoundUser(true, CurrentUser);
        }

        public void SaveUserAsync(string firstName, string lastName)
        {
            if (!ValidValues(firstName, lastName)) return;

            TextToSpeechManager.Instance.SpeakText("Hey " + firstName + ", you must be new around here. Lets create a new profile for you."); // TODO: move speech into scene manager

            var user = new UserProfile();
            user.first = firstName;
            user.last = lastName;

            StartCoroutine( AppServicesManager.Instance.User.Insert<UserProfile>(user, SaveUserComplete) );
        }

        private void SaveUserComplete(IRestResponse<UserProfile> response)
        {
            if (response.IsError)
            {
                TextToSpeechManager.Instance.SpeakText("Well this is embrassing, just can't seem to save your user profile right now..."); // TODO: move speech into scene manager
                OnSavedUser(false, CurrentUser);
                return;
            }
            CurrentUser = response.Data;
            OnSavedUser(true, CurrentUser);
        }

        private bool ValidValues(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                Debug.LogError("First and last name are required.");
                return false;
            }
            return true;
        }

    }
}
