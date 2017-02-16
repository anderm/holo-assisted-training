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
        // callbacks for async methods, where is await when you need it
        private Action<bool, UserProfile[]> OnLoadedUsers;
        private Action<bool, UserProfile> OnFoundUser;
        private Action<bool, UserProfile> OnSavedUser;
        private Action<bool, UserProfile> OnChangedUser;

        public string FirstName { get; set; }
        public string LastName { get; set; }

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
        }

        public void LoadUsersAsync(Action<bool, UserProfile[]> callback = null)
        {
            OnLoadedUsers = callback;
            StartCoroutine(AppServicesManager.Instance.User.Read<UserProfile>(LoadUsersComplete));
        }

        private void LoadUsersComplete(IRestResponse<UserProfile[]> response)
        {
            if (response.IsError)
            {
                if (OnLoadedUsers != null)
                {
                    OnLoadedUsers(false, null);
                }

                return;
            }

            UserProfiles = response.Data;
            if (OnLoadedUsers != null)
            {
                OnLoadedUsers(true, UserProfiles);
            }
        }

        public bool TryRetrieveUser(string firstName, string lastName)
        {
            if (UserProfiles == null || UserProfiles.Length == 0 || !ValidValues(firstName, lastName))
            {
                return false;
            }

            foreach (UserProfile user in UserProfiles)
            {
                if (user.first.Equals(firstName, StringComparison.OrdinalIgnoreCase) && user.last.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                {
                    CurrentUser = user;
                    return true;
                }
            }

            return false;
        }

        public void TryFindUserAsync(string firstName, string lastName, Action<bool, UserProfile> callback)
        {
            CurrentUser = new UserProfile() { first = firstName, last = lastName };

            if (!ValidValues(firstName, lastName))
            {
                callback(false, CurrentUser);
                return;
            }

            // Try to find from loaded profiles first (limited to 50 users by default)
            if (TryRetrieveUser(firstName, lastName))
            {
                callback(true, CurrentUser);
                return;
            }

            string filter = string.Format("first eq '{0}' and last eq '{1}'", firstName, lastName);
            CustomQuery query = new CustomQuery(filter);
            StartCoroutine(AppServicesManager.Instance.User.Query<UserProfile>(query, TryFindUserComplete));
            OnFoundUser = callback;
        }

        private void TryFindUserComplete(IRestResponse<UserProfile[]> response)
        {
            if (response.IsError || response.Data.Length == 0)
            {
                OnFoundUser(false, CurrentUser);
                return;
            }

            if (response.Data.Length != 1)
            {
                Debug.LogWarning("Found multiple profiles, did you clone yourself?"); // TODO: handle persons which have duplicate names
            }

            CurrentUser = response.Data[0];
            OnFoundUser(true, CurrentUser);
        }

        public void SaveUserAsync(string firstName, string lastName, Action<bool, UserProfile> callback)
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
            StartCoroutine(AppServicesManager.Instance.User.Insert<UserProfile>(user, SaveUserComplete));
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
            return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }
    }
}
