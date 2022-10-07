using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System;
using System.Linq;

using static DataAccess.Authenticator;

namespace Tests {
    [TestClass]
    public class AuthenticatorTests {
        // super secure password
        private static String superPassword = "password";
        private static String superPasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";

        // dummy users
        private static String authTestsUser = "ImAUser";
        private static String registerTestsUser = "tacosdemango";
        private static String unregisterTestsUser = "tOdiomyriam";

        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            // Create dummy users
            var dbContext = new DataAccess.Scrabble99Entities();
            var player = new DataAccess.players();
            player.password = superPasswordHash;

            player.username = authTestsUser;
            dbContext.players.Add(player);

            player.username = unregisterTestsUser;
            dbContext.players.Add(player);

            dbContext.SaveChanges();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            // Set up db context
            var dbContext = new DataAccess.Scrabble99Entities();

            Func<String, bool> unregisterDummyUser = username => {
                try {
                    var player = dbContext.players.Where(p => p.username == username).First();
                    dbContext.players.Remove(player);
                    dbContext.SaveChanges();
                    return true;
                }
                catch(Exception ex) {
                    return false;
                }
            };

            if(!(unregisterDummyUser(authTestsUser) && unregisterDummyUser(registerTestsUser))) {
                throw new Exception("failed to clean up");
            }
        }

        [TestMethod]
        public void testHashPassword() {
            var authtenticator = new PrivateType(typeof(Authenticator));
            String hashedPassword = authtenticator.InvokeStatic("hashPassword", superPassword) as String;
            Assert.AreEqual(superPasswordHash, hashedPassword);
        }

        [TestMethod]
        public void testCheckPassword() {
            var authtenticator = new PrivateType(typeof(Authenticator));
            var passwordCheckResult = (bool)authtenticator.InvokeStatic("checkPassword", superPassword, superPasswordHash);
            Assert.IsTrue(passwordCheckResult);
        }

        [TestMethod]
        public void testLoginSuccess() {
            var loginResult = Authenticator.validateUser(authTestsUser, superPassword);
            Assert.AreEqual(UserAuthResult.Success, loginResult);
        }

        [TestMethod]
        public void testLoginInvalidCredentials() {
            String nonexistentUser = "tacosdemango";
            var loginResult = Authenticator.validateUser(nonexistentUser, superPassword);
            Assert.AreEqual(UserAuthResult.InvalidCredentials, loginResult);
        }

        [TestMethod]
        public void testLoginIncorrectPassword() {
            String wrongPassword = "wrongPassword";
            var loginResult = Authenticator.validateUser(authTestsUser, wrongPassword);
            Assert.AreEqual(UserAuthResult.IncorrectPassword, loginResult);
        }

        [TestMethod]
        public void testRegisterSuccess() {
            String somePassword = "passwd123";
            var registerResult = Authenticator.registerUser(registerTestsUser, somePassword);
            Assert.AreEqual(UserResgisterResult.Success, registerResult);
        }

        [TestMethod]
        public void testRegisterUserAlreadyExists() {
            var registerResult = Authenticator.registerUser(authTestsUser, superPassword);
            Assert.AreEqual(UserResgisterResult.UserAlreadyExists, registerResult);
        }

        [TestMethod]
        public void testUnregisterSuccess() {
            var unregisterResult = Authenticator.unregisterUser(unregisterTestsUser, superPassword);
            Assert.AreEqual(UserUnregisterResult.Success, unregisterResult);
        }

        [TestMethod]
        public void testUnregisterUserDoesNotExist() {
            String nonexistentUser = "tacossinmango";
            var unregisterResult = Authenticator.unregisterUser(nonexistentUser, superPassword);
            Assert.AreEqual(UserUnregisterResult.UserDoesNotExists, unregisterResult);
        }

        [TestMethod]
        public void testUnregisterAuthFailed() {
            var unregisterResult = Authenticator.unregisterUser(authTestsUser, "wrongPassword");
            Assert.AreEqual(UserUnregisterResult.AuthFailed, unregisterResult);
        }
    }
}
