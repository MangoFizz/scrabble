using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Core;
using DataAccess;

using static Core.PlayerManager;

namespace Tests {
    [TestClass]
    public class AuthenticatorTests {
        // super secure password
        private static string superPassword = "password";
        private static string superPasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";

        // dummy users
        private static string authTestsUser = "ImAUser";
        private static string registerTestsUser = "tacosdemango";
        private static string unregisterTestsUser = "tOdiomyriam";

        // email
        private static string email = "holamundo@uv.mx";

        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            // Create dummy users
            var dbContext = new ScrabbleEntities();
            var player = new Player();
            player.Password = superPasswordHash;

            player.Nickname = authTestsUser;
            dbContext.players.Add(player);

            player.Nickname = unregisterTestsUser;
            dbContext.players.Add(player);

            dbContext.SaveChanges();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            // Set up db context
            var dbContext = new ScrabbleEntities();

            Func<string, bool> unregisterDummyUser = username => {
                try {
                    var player = dbContext.players.Where(p => p.Nickname == username).First();
                    dbContext.players.Remove(player);
                    dbContext.SaveChanges();
                    return true;
                }
                catch {
                    return false;
                }
            };

            if(!(unregisterDummyUser(authTestsUser) && unregisterDummyUser(registerTestsUser))) {
                throw new Exception("failed to clean up");
            }
        }

        [TestMethod]
        public void TestHashPassword() {
            var authtenticator = new PrivateType(typeof(PlayerManager));
            string hashedPassword = authtenticator.InvokeStatic("hashPassword", superPassword) as string;
            Assert.AreEqual(superPasswordHash, hashedPassword);
        }

        [TestMethod]
        public void TestCheckPassword() {
            var authtenticator = new PrivateType(typeof(PlayerManager));
            var passwordCheckResult = (bool)authtenticator.InvokeStatic("checkPassword", superPassword, superPasswordHash);
            Assert.IsTrue(passwordCheckResult);
        }

        [TestMethod]
        public void TestLoginSuccess() {
            var loginResult = AuthenticatePlayer(authTestsUser, superPassword);
            Assert.AreEqual(PlayerAuthResult.Success, loginResult);
        }

        [TestMethod]
        public void TestLoginInvalidCredentials() {
            string nonexistentUser = "tacosdemango";
            var loginResult = AuthenticatePlayer(nonexistentUser, superPassword);
            Assert.AreEqual(PlayerAuthResult.InvalidCredentials, loginResult);
        }

        [TestMethod]
        public void TestLoginIncorrectPassword() {
            string wrongPassword = "wrongPassword";
            var loginResult = AuthenticatePlayer(authTestsUser, wrongPassword);
            Assert.AreEqual(PlayerAuthResult.IncorrectPassword, loginResult);
        }

        [TestMethod]
        public void TestRegisterSuccess() {
            string somePassword = "passwd123";
            var registerResult = RegisterPlayer(registerTestsUser, somePassword, email);
            Assert.AreEqual(PlayerResgisterResult.Success, registerResult);
        }

        [TestMethod]
        public void TestRegisterUserAlreadyExists() {
            var registerResult = RegisterPlayer(authTestsUser, superPassword, email);
            Assert.AreEqual(PlayerResgisterResult.PlayerAlreadyExists, registerResult);
        }

        [TestMethod]
        public void TestUnregisterSuccess() {
            var unregisterResult = UnregisterPlayer(unregisterTestsUser, superPassword);
            Assert.AreEqual(PlayerUnregisterResult.Success, unregisterResult);
        }

        [TestMethod]
        public void TestUnregisterUserDoesNotExist() {
            string nonexistentUser = "tacossinmango";
            var unregisterResult = UnregisterPlayer(nonexistentUser, superPassword);
            Assert.AreEqual(PlayerUnregisterResult.PlayerDoesNotExists, unregisterResult);
        }

        [TestMethod]
        public void TestUnregisterAuthFailed() {
            var unregisterResult = UnregisterPlayer(authTestsUser, "wrongPassword");
            Assert.AreEqual(PlayerUnregisterResult.AuthFailed, unregisterResult);
        }
    }
}
