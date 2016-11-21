using Assets.Code.Models;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Managers
{
    public class LoginManager : Singleton<LoginManager>
    {
        private Dictionary<string, User> usersDictionary;

        public LoginManager()
        {
            usersDictionary = new Dictionary<string, User>();

            var mockUser = new User() { FirstName = "John", LastName = "Douglas", FullName = "John Douglas", LastAttemptTime = 90, LastScore = 500, UserId = Guid.NewGuid() };
            usersDictionary.Add(mockUser.FullName, mockUser);
        }

        private void addUser(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.FullName))
            {
                this.usersDictionary.Add(user.FullName, user);
            }
        }

        public User TryRetrieveUser(string fullName)
        {
            User user = null;

            usersDictionary.TryGetValue(fullName, out user);

            return user;
        }

        public Guid AddUser(string firstName, string lastName)
        {
            var user = new User();

            user.FirstName = firstName;
            user.LastName = lastName;
            user.FullName = firstName + " " + lastName;

            user.UserId = Guid.NewGuid();

            this.usersDictionary.Add(user.FullName, user);

            return user.UserId;
        }
    }
}
