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
        public static Action<bool, UserProfile> OnSavedUser;

        public delegate void ChangedUser(UserProfile userProfile);
        public static event ChangedUser OnChangedUser;

        private UserProfile[] UserProfiles;
        private UserProfile currentUser;
        public UserProfile CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
            }
        }

        public LoginManager()
        {
        }

        void Start()
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
                return;
            }
            Debug.Log(response.StatusCode);
            Debug.Log(response.ErrorMessage + response.Content);

            if(response.Data == null)
            {
                Debug.Log("Data is null :(");
            }

            UserProfiles = response.Data;
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

        public void SaveUserAsync(string firstName, string lastName, Action<bool, UserProfile> callback = null)
        {
            if (!ValidValues(firstName, lastName))
            {
                callback(false, null);
                return;
            }

            var user = new UserProfile();
            user.first = firstName;
            user.last = lastName;

            OnSavedUser = callback;
            StartCoroutine( AppServicesManager.Instance.User.Insert<UserProfile>(user, SaveUserComplete) );
        }

        private void SaveUserComplete(IRestResponse<UserProfile> response)
        {
            if (response.IsError)
            {
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
