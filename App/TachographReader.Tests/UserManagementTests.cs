using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Webcal.DataModel.Repositories;
using UserManager = Webcal.DataModel.Library.UserManagement;

namespace TachographReader.Tests
{
    [TestFixture]
    public class UserManagementTests
    {
        [Test]
        public void Encrypt_PasswordIsEmpty_ReturnsEmptyString()
        {
            string password = string.Empty;

            string result = UserManager.Encrypt(password);

            Assert.That(string.IsNullOrEmpty(result));
        }

        [Test]
        public void Encrypt_SimplePassword_ReturnsValidHash()
        {
            string password = "password";
            string expectedResult = "^ˆH˜Ú(qQÐåoÆ)'s`=\rj«½Ö*ïrBØ";

            string result = UserManager.Encrypt(password);

            Assert.That(string.Equals(result, expectedResult));
        }

        [Test]
        public void Encrypt_StrongerPassword_ReturnsValidHash()
        {
            string password = "p@55w0rd";
            string expectedResult = "Yôk¹\fÿ°í|~]µ‹³\0ó¼×õç#í‘°j>ÔÕ¶";

            string result = UserManager.Encrypt(password);

            Assert.That(string.Equals(result, expectedResult));
        }

        [Test]
        public void Encrypt_StrongestPassword_ReturnsValidHash()
        {
            string password = "You take the red pill, you stay in Wonderland, and I show you how deep the rabbit hole goes.";
            string expectedResult = "6m9F'Ô‹ê×M|ØQ£p¹¸ô†§Œ9êö¤q­";

            string result = UserManager.Encrypt(password);

            Assert.That(string.Equals(result, expectedResult));
        }

        [Test]
        public void Validate_UsernameIsEmpty_ReturnsFalse()
        {
            string username = string.Empty;
            string password = string.Empty;
            UserRepository repository = null;

            bool valid = UserManager.Validate(repository, username, password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_PasswordIsEmpty_ReturnsFalse()
        {
            string username = "Jon";
            string password = string.Empty;
            UserRepository repository = null;

            bool valid = UserManager.Validate(repository, username, password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_RepositoryIsNull_ReturnsFalse()
        {
            string username = "Jon";
            string password = "Password";
            UserRepository repository = null;

            bool valid = UserManager.Validate(repository, username, password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void Validate_UserIsSuperUserValidPassword_ReturnsTrue()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "superuser", Password = "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY" });

            string username = "superuser";
            string password = "yellowskillweb";

            bool isValid = UserManager.Validate(repository, username, password);

            Assert.That(isValid);
        }

        [Test]
        public void Validate_UserIsSuperUserInvalidPassword_ReturnsTrue()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "superuser", Password = "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY" });

            string username = "superuser";
            string password = "silly";

            bool isValid = UserManager.Validate(repository, username, password);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_UsernameIsInvalid_ReturnsFalse()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "superuser", Password = "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY" });

            string username = "superssuser";
            string password = "yellowskillweb";

            bool isValid = UserManager.Validate(repository, username, password);

            Assert.IsFalse(isValid);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException))]
        public void AddSuperUser_RepositoryIsNull_ThrowsInvalidOperationException()
        {
            UserRepository repository = null;

            UserManager.AddSuperUser(repository);

            //Expected exception
        }

        [Test]
        public void AddSuperUser_RepositoryIsNotNull_UserIsAddedToRepository()
        {
            var repository = new MockUserRepository();

            UserManager.AddSuperUser(repository);

            Assert.That(repository.GetAll().Count == 1);
            var first = repository.GetAll().First();
            Assert.That(first.Username == "superuser");
            Assert.That(first.Password == "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY"); //yellowskillweb
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException))]
        public void AddUser_RepositoryIsNull_ThrowsInvalidOperationException()
        {
            UserRepository repository = null;
            string username = string.Empty;
            string password = string.Empty;

            UserManager.AddUser(repository, username, password);

            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void AddUser_UsernameIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = string.Empty;
            string password = string.Empty;

            UserManager.AddUser(repository, username, password);

            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void AddUser_PasswordIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = "user";
            string password = string.Empty;

            UserManager.AddUser(repository, username, password);

            //Expected exception
        }

        [Test]
        public void AddUser_UsernameAndPasswordAreValid_AddsNewUserToRepository()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = "user";
            string password = "password";

            UserManager.AddUser(repository, username, password);

            Assert.That(repository.GetAll().Count == 1);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void UserExists_RepositoryIsNull_ThrowsArgumentNullException()
        {
            MockUserRepository repository = null;
            string username = string.Empty;

            bool exists = UserManager.UserExists(repository, username);

            //Expected exception
        }

        [Test]
        public void UserExists_UsernameIsEmpty_ReturnsTrue()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = string.Empty;

            bool exists = UserManager.UserExists(repository, username);

            Assert.IsTrue(exists);
        }

        [Test]
        public void UserExists_UsernameDoesNotExist_ReturnsFalse()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = "user";

            bool exists = UserManager.UserExists(repository, username);

            Assert.IsFalse(exists);
        }

        [Test]
        public void UserExists_UsernameDoesExist_ReturnsFalse()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "User" });
            string username = "User";

            bool exists = UserManager.UserExists(repository, username);

            Assert.IsTrue(exists);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void ChangePassword_RepositoryIsNull_ThrowsArgumentNullException()
        {
            MockUserRepository repository = null;
            UserManager.LoggedInUserName = string.Empty;
            string oldPassword = string.Empty;
            string newPassword = string.Empty;

            UserManager.ChangePassword(repository, oldPassword, newPassword);

            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void ChangePassword_UsernameIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            UserManager.LoggedInUserName = string.Empty;
            string oldPassword = string.Empty;
            string newPassword = string.Empty;

            UserManager.ChangePassword(repository, oldPassword, newPassword);

            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void ChangePassword_OldPasswordIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            UserManager.LoggedInUserName = "User";
            string oldPassword = string.Empty;
            string newPassword = string.Empty;

            UserManager.ChangePassword(repository, oldPassword, newPassword);

            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void ChangePassword_NewPasswordIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = null;
            UserManager.LoggedInUserName = "User";
            string oldPassword = "OldPassword";
            string newPassword = string.Empty;

            UserManager.ChangePassword(repository, oldPassword, newPassword);

            //Expected exception
        }

        [Test]
        public void ChangePassword_OldPasswordIsWrong_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "User", Password = "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY" });

            UserManager.LoggedInUserName = "User";
            string oldPassword = "oldpassword";
            string newPassword = "newpassword";

            bool hasChanged = UserManager.ChangePassword(repository, oldPassword, newPassword);

            Assert.IsFalse(hasChanged);
        }

        [Test]
        public void ChangePassword_OldPasswordIsCorrect_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "User", Password = "–6¬•Å0³½v‰:•Z;í½z*Ë¼dÌ¢MMðÃ4QY" });

            UserManager.LoggedInUserName = "User";
            string oldPassword = "yellowskillweb";
            string newPassword = "newpassword";

            bool hasChanged = UserManager.ChangePassword(repository, oldPassword, newPassword);

            Assert.IsTrue(hasChanged);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void GetUser_RepositoryIsNull_ThrowsArgumentNullException()
        {
            MockUserRepository repository = null;
            string username = string.Empty;

            var user = UserManager.GetUser(repository, username);
            
            //Expected exception
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException))]
        public void GetUser_UsernameIsEmpty_ThrowsArgumentNullException()
        {
            MockUserRepository repository = new MockUserRepository();
            string username = string.Empty;

            var user = UserManager.GetUser(repository, username);

            //Expected exception
        }

        [Test]
        public void GetUser_ValidUsername_ReturnsUser()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User {Username = "Jon"});
            string username = "Jon";
            
            var user = UserManager.GetUser(repository, username);

            Assert.That(string.Equals(user.Username, username));
        }

        [Test]
        public void GetUser_InvalidUsername_ReturnsNull()
        {
            MockUserRepository repository = new MockUserRepository();
            repository.Add(new Webcal.DataModel.User { Username = "Paul" });
            string username = "Jon";

            var user = UserManager.GetUser(repository, username);

            Assert.That(user == null);
        }

        #region Helper Methods

        public static string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            return StringCipher.GetPasswordHash(password);
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

        #endregion
    }
}
