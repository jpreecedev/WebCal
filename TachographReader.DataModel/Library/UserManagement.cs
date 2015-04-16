namespace TachographReader.DataModel.Library
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Core;
    using Properties;
    using Shared;

    public static class UserManagement
    {
        public static string LoggedInUserName { get; set; }
        public static IUserSignatureCapture SelectedUser { get; set; }
        public static DateTime LastCommandExecuted { get; set; }

        public static bool LoggedInAsSuperUser
        {
            get { return string.Equals("superuser", LoggedInUserName, StringComparison.CurrentCultureIgnoreCase); }
        }

        public static bool Validate(IRepository<User> repository, string username, string password)
        {
            if (repository == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            return repository.FirstOrDefault(t => string.Equals(t.Username, username, StringComparison.CurrentCultureIgnoreCase) && string.Equals(t.Password, Encrypt(password))) != null;
        }

        public static bool UserExists(IRepository<User> repository, string username)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (string.IsNullOrEmpty(username))
            {
                return true;
            }

            return repository.FirstOrDefault(t => string.Equals(t.Username, username, StringComparison.CurrentCultureIgnoreCase)) != null;
        }

        public static void AddUser(IRepository<User> repository, string username, string password)
        {
            if (repository == null)
            {
                throw new InvalidOperationException(Resources.EXC_USER_REPOSITORY_IS_REQUIRED);
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException();
            }

            repository.Add(new User
            {
                Username = username,
                Password = Encrypt(password)
            });
        }

        public static void RemoveUser(IRepository<User> repository, string username)
        {
            if (repository == null)
            {
                throw new InvalidOperationException(Resources.EXC_USER_REPOSITORY_IS_REQUIRED);
            }
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException();
            }

            repository.Remove(new User
            {
                Username = username
            });
        }

        public static bool ChangePassword(IRepository<User> repository, string oldPassword, string newPassword)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentNullException("oldPassword");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("newPassword");
            }

            User user = repository.FirstOrDefault(u => string.Equals(LoggedInUserName, u.Username, StringComparison.CurrentCultureIgnoreCase));
            if (user == null)
            {
                return false;
            }

            if (string.Equals(user.Password, Encrypt(oldPassword)))
            {
                user.Password = Encrypt(newPassword);
                repository.Save();

                return true;
            }
            return false;
        }

        public static bool ResetPassword(IRepository<User> repository, string oldPassword, string newPassword)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentNullException("oldPassword");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("newPassword");
            }

            User user = repository.FirstOrDefault(u => string.Equals(LoggedInUserName, u.Username, StringComparison.CurrentCultureIgnoreCase));
            if (user == null)
            {
                return false;
            }

            if (string.Equals(((User)SelectedUser).Password, oldPassword))
            {
                ((User)SelectedUser).Password = Encrypt(newPassword);
                repository.Save();

                return true;
            }
            return false;
        }

        public static void AddSuperUser(IRepository<User> repository)
        {
            if (repository == null)
            {
                throw new InvalidOperationException(Resources.EXC_USER_REPOSITORY_IS_REQUIRED);
            }

            repository.Add(new User
            {
                Username = "superuser",
                Password = "°ŒîÈL±8É‰‘kÔ ‚åUrHºðlTo*ùÎ¸¥·úê"
            });
        }

        public static void UpdateSuperUser(IRepository<User> userRepository, User superUser)
        {
            if (userRepository == null)
            {
                throw new ArgumentNullException("userRepository");
            }
            if (superUser == null)
            {
                throw new ArgumentNullException("superUser");
            }

            if (!string.Equals("°ŒîÈL±8É‰‘kÔ ‚åUrHºðlTo*ùÎ¸¥·úê", superUser.Password))
            {
                superUser.Password = "°ŒîÈL±8É‰‘kÔ ‚åUrHºðlTo*ùÎ¸¥·úê";
            }

            userRepository.Save();
        }

        public static bool HasTimedOut()
        {
            DateTime now = DateTime.Now;
            DateTime lastCommandExecuted = LastCommandExecuted;

            return now - lastCommandExecuted > new TimeSpan(0, 5, 0);
        }

        public static string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            return StringCipher.GetPasswordHash(password);
        }

        public static User GetUser(IRepository<User> repository, string userName)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            return repository.FirstOrDefault(user => string.Equals(user.Username, userName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static void AddDefaultUser()
        {
            var repository = ContainerBootstrapper.Resolve<IRepository<User>>();

            if (repository != null)
            {
                if (!UserExists(repository, "tacho"))
                {
                    AddUser(repository, "tacho", "tacho");
                    repository.Save();
                }
            }
        }

        private static class StringCipher
        {
            public static string GetPasswordHash(string password)
            {
                byte[] data = Encoding.Default.GetBytes(password);

                SHA256 shaM = new SHA256Managed();
                byte[] result = shaM.ComputeHash(data);

                return Encoding.Default.GetString(result);
            }
        }
    }
}