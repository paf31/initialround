///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using InitialRound.BusinessLogic.Properties;
using System.Globalization;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.Common.Extensions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.Models.Contexts;
using System.Data.SqlClient;
using System.IO;
using InitialRound.Models.Schema.dbo;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class UserController
    {
        public static AuthToken ValidateUser(string username, string password, bool requireActiveAccount)
        {
            DbContext context = DataController.CreateDbContext();

            User user = context.Users.FirstOrDefault(u => u.ID == username);

            if (user == null)
            {
                throw new AuthenticationException("Invalid login.");
            }

            if (DateTime.UtcNow - user.LastLoginDate < Settings.Default.MinTimeBetweenLoginAttempts)
            {
                throw new AuthenticationException(string.Format("Please wait at least {0} seconds between login attempts.", Settings.Default.MinTimeBetweenLoginAttempts.Seconds));
            }

            if (DateTime.UtcNow - user.LastLoginDate < Settings.Default.AccountLockDuration && user.LoginAttempts > Settings.Default.MaxLoginAttempts)
            {
                throw new AuthenticationException(string.Format("Your account has been locked for {0} minutes due to too many incorrect login attempts.", Settings.Default.AccountLockDuration.Minutes));
            }

            byte[] passwordHash = user.PasswordHash;

            byte[] computedHash = SaltAndHashPassword(password, user.PasswordSalt);

            bool isValid = Enumerable.SequenceEqual(computedHash, passwordHash);

            user.LastLoginDate = DateTime.UtcNow;

            AuthToken token = null;

            if (isValid)
            {
                DateTime expiresOn = DateTime.UtcNow + Settings.Default.SessionExpiryInterval;

                token = new AuthToken(username, GetCallerIPAddress(), expiresOn, user.IsAdmin,
                    Common.Helpers.RandomHelper.RandomLong());

                user.LoginAttempts = 0;
                context.SaveChanges();
            }
            else
            {
                user.LoginAttempts++;
                context.SaveChanges();

                if (user.LoginAttempts < Settings.Default.MaxLoginAttempts)
                {
                    throw new AuthenticationException("Invalid login.");
                }
                else
                {
                    throw new AuthenticationException(string.Format("Your account has been locked for {0} minutes due to too many incorrect login attempts.",
                        Settings.Default.AccountLockDuration.Minutes));
                }
            }

            return token;
        }

        public static AuthToken KeepSessionAlive(AuthToken token)
        {
            DateTime expiresOn = DateTime.UtcNow + Settings.Default.SessionExpiryInterval;

            return new AuthToken(token.Username, GetCallerIPAddress(), expiresOn, token.IsAdmin, Common.Helpers.RandomHelper.RandomLong());
        }

        public static void UpdateUser(string username, string firstName, string lastName, string emailAddress, AuthToken token)
        {
            if (!token.IsAdmin)
            {
                throw new AuthenticationException("Admin must perform this action");
            }

            DbContext context = DataController.CreateDbContext();

            User user = context.Users.FirstOrDefault(u => u.ID == username);

            if (user == null)
            {
                throw new AuthenticationException("User does not exist!");
            }

            if (EmailAddressExists(emailAddress, username))
            {
                throw new AuthenticationException("Email address is already in use");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.EmailAddress = emailAddress;

            context.SaveChanges();
        }

        public static void DeleteUser(string username, AuthToken token)
        {
            if (!token.IsAdmin)
            {
                throw new AuthenticationException("Admin must perform this action");
            }

            DbContext context = DataController.CreateDbContext();

            User user = context.Users.FirstOrDefault(u => u.ID == username);

            if (user == null)
            {
                throw new AuthenticationException("User does not exist!");
            }

            if (user.IsAdmin)
            {
                throw new AuthenticationException("Cannot delete the admin user!");
            }

            context.Users.Remove(user);
            context.SaveChanges();
        }

        public static string CreateSessionCookie(AuthToken token)
        {
            string encryptedToken = Convert.ToBase64String(EncryptionHelper.EncryptToken(token.AsBytes()));

            return HttpUtility.UrlEncode(encryptedToken);
        }

        public static string NewAntiForgeryToken(string username)
        {
            AntiForgeryToken antiForgeryToken = new AntiForgeryToken(username, GetCallerIPAddress(),
                DateTime.UtcNow + Settings.Default.AntiForgeryTokenExpiryInterval,
                Common.Helpers.RandomHelper.RandomLong());

            return Convert.ToBase64String(EncryptionHelper.EncryptAntiForgeryToken(antiForgeryToken.AsBytes()));
        }

        private static byte[] HashPasswordGeneratingSalt(string password, out byte[] passwordSalt)
        {
            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, Settings.Default.SaltSize))
            {
                deriveBytes.IterationCount = Settings.Default.HashIterations;
                passwordSalt = deriveBytes.Salt;
                return deriveBytes.GetBytes(Settings.Default.HashSize);
            }
        }

        private static byte[] SaltAndHashPassword(string password, byte[] passwordSalt)
        {
            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, passwordSalt))
            {
                deriveBytes.IterationCount = Settings.Default.HashIterations;
                return deriveBytes.GetBytes(Settings.Default.HashSize);
            }
        }

        private static string GetCallerIPAddress()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        public static bool ValidateSession(string token, out AuthToken authToken)
        {
            byte[] tokenBytes = EncryptionHelper.DecryptToken(Convert.FromBase64String(token));

            using (MemoryStream memoryStream = new MemoryStream(tokenBytes))
            {
                authToken = AuthToken.Deserialize(memoryStream);
            }

            return ValidateAuthToken(authToken, Settings.Default.SessionExpiryInterval);
        }

        public static bool ValidateAntiForgeryToken(string token, out AntiForgeryToken antiForgeryToken)
        {
            byte[] tokenBytes = EncryptionHelper.DecryptAntiForgeryToken(Convert.FromBase64String(token));

            using (MemoryStream memoryStream = new MemoryStream(tokenBytes))
            {
                antiForgeryToken = AntiForgeryToken.Deserialize(memoryStream);
            }

            return ValidateAuthToken(antiForgeryToken, Settings.Default.AntiForgeryTokenExpiryInterval);
        }

        public static bool ValidateAuthToken(AntiForgeryToken antiForgeryToken, TimeSpan validityPeriod)
        {
            if (DateTime.UtcNow > antiForgeryToken.ExpiresOn)
            {
                return false;
            }

            string callerIPAddress = GetCallerIPAddress();

            if (!string.Equals(callerIPAddress, antiForgeryToken.IPAddress))
            {
                return false;
            }

            return true;
        }

        public static void SendPasswordResetEmail(string username)
        {
            DbContext context = DataController.CreateDbContext();

            var userInfo = (from user in context.Users
                            where user.ID == username
                            select new
                            {
                                user.FirstName,
                                user.LastName,
                                user.EmailAddress,
                                UserName = user.ID
                            }).FirstOrDefault();

            if (userInfo == null)
            {
                throw new AuthenticationException("User not found.");
            }

            EmailController.SendPasswordResetEmail(
                                userInfo.FirstName,
                                userInfo.LastName,
                                userInfo.EmailAddress,
                                userInfo.UserName);
        }

        public static void CreateUser(string username, string password, string emailAddress, string firstName, string lastName, AuthToken token, bool isAdmin)
        {
            if (!token.IsAdmin)
            {
                throw new AuthenticationException("Admin must perform this action");
            }

            DbContext context = DataController.CreateDbContext();

            if (UserExists(username))
            {
                throw new AuthenticationException("Username is taken.");
            }

            if (EmailAddressExists(emailAddress))
            {
                throw new AuthenticationException("Email address is already in use.");
            }

            CreateUser(context, username, password, emailAddress, firstName, lastName, isAdmin);

            context.SaveChanges();

            EmailController.SendNewUserEmail(firstName, lastName, username, emailAddress);
        }

        public static void CreateUser(DbContext context, string username, string password, string emailAddress, string firstName, string lastName, bool isAdmin)
        {
            User user = new User();

            user.ID = username;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.EmailAddress = emailAddress;
            user.CreatedDate = DateTime.UtcNow;
            user.LastUpdatedDate = DateTime.UtcNow;
            user.IsAdmin = isAdmin;

            byte[] salt;

            user.PasswordHash = HashPasswordGeneratingSalt(password, out salt);
            user.PasswordSalt = salt;

            context.Users.Add(user);
        }

        private static bool UserExists(string username)
        {
            DbContext context = DataController.CreateDbContext();

            return context.Users.Any(u => u.ID == username);
        }

        private static bool EmailAddressExists(string emailAddress, string username = null)
        {
            DbContext context = DataController.CreateDbContext();

            if (username == null)
            {
                return context.Users.Any(u => u.EmailAddress == emailAddress);
            }
            else
            {
                return context.Users.Any(u => u.ID != username && u.EmailAddress == emailAddress);
            }
        }

        public static void ChangePassword(string username, string oldPassword, string newPassword)
        {
            DbContext context = DataController.CreateDbContext();

            User user = context.Users.FirstOrDefault(u => u.ID == username);

            if (user == null)
            {
                throw new AuthenticationException("User does not exist!");
            }

            byte[] passwordHash = user.PasswordHash;

            byte[] computedHash = SaltAndHashPassword(oldPassword, user.PasswordSalt);

            if (!Enumerable.SequenceEqual(computedHash, passwordHash))
            {
                throw new AuthenticationException("Old password is incorrect.");
            }

            ChangePassword(context, user, newPassword);
        }

        public static void ChangePassword(string username, string newPassword)
        {
            DbContext context = DataController.CreateDbContext();

            User user = context.Users.FirstOrDefault(u => u.ID == username);

            if (user == null)
            {
                throw new AuthenticationException("User does not exist!");
            }

            ChangePassword(context, user, newPassword);
        }

        private static void ChangePassword(DbContext context, User user, string newPassword)
        {
            byte[] salt;

            user.PasswordHash = HashPasswordGeneratingSalt(newPassword, out salt);
            user.PasswordSalt = salt;

            context.SaveChanges();
        }

        public static void ValidateAntiForgeryToken(string tokenString, AuthToken authToken)
        {
            AntiForgeryToken antiForgeryToken;

            if (!ValidateAntiForgeryToken(tokenString, out antiForgeryToken))
            {
                throw new AuthenticationException("Invalid token.");
            }

            if (!string.Equals(antiForgeryToken.Username, authToken.Username))
            {
                throw new AuthenticationException("Invalid token.");
            }
        }

        public static void ClearAuthCookies(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies[Constants.AuthToken] != null)
            {
                HttpCookie authToken = new HttpCookie(Constants.AuthToken);
                authToken.Expires = DateTime.Now.AddDays(-1d);
                response.Cookies.Add(authToken);
            }

            if (request.Cookies[Constants.Expires] != null)
            {
                HttpCookie authToken = new HttpCookie(Constants.Expires);
                authToken.Expires = DateTime.Now.AddDays(-1d);
                response.Cookies.Add(authToken);
            }
        }
    }
}

