using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Core;
using DataAccess;

using static Core.PlayerManager;

namespace Tests {
    [TestClass]
    public class AuthenticatorTests {
        private static string InsecurePassword = "password";
        private static string InsecurePasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";
        private static string SecurePassword = "Password123.";
        private static string DummyUserNickname1 = "DummyUser1";
        private static string DummyUserNickname2 = "DummyUser2";
        private static string DummyUserNickname3 = "DummyUser3";
        private static string DummyUserNickname4 = "DummyUser4";
        private static string DummyUserNickname5 = "tortilla";
        private static string DummyUserNickname6 = "helado";
        private static string Email = "holamundo@uv.mx";

        private static Player DummyPlayer1 { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            var dbContext = new ScrabbleEntities();

            DummyPlayer1 = dbContext.players.Add(new Player() {
                Nickname = DummyUserNickname1,
                Password = HashPassword(SecurePassword),
                Email = Email,
                Registered = DateTime.Now,
                VerificationCode = GenerateVerificationCode(),
                Verified = false
            });

            var DummyPlayer2 = dbContext.players.Add(new Player() {
                Nickname = DummyUserNickname2,
                Password = HashPassword(SecurePassword),
                Email = Email,
                Registered = DateTime.Now,
                VerificationCode = GenerateVerificationCode(),
                Verified = true
            });

            var DummyPlayer4 = dbContext.players.Add(new Player() {
                Nickname = DummyUserNickname4,
                Password = HashPassword(SecurePassword),
                Email = Email,
                Registered = DateTime.Now,
                VerificationCode = GenerateVerificationCode(),
                Verified = false
            });

            var DummyPlayer5 = dbContext.players.Add(new Player() {
                Nickname = DummyUserNickname5,
                Password = HashPassword(SecurePassword),
                Email = Email,
                Registered = DateTime.Now,
                VerificationCode = GenerateVerificationCode(),
                Verified = true
            });

            dbContext.SaveChanges();

            RequestFriendship(DummyUserNickname2, DummyUserNickname4);
            RequestFriendship(DummyUserNickname4, DummyUserNickname1);
            RequestFriendship(DummyUserNickname4, DummyUserNickname5);
            AnswerFriendshipRequest(DummyUserNickname5, DummyUserNickname4, true);
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            var dbContext = new ScrabbleEntities();
            var DummyPlayer1 = dbContext.players.First(p => p.Nickname == DummyUserNickname1);
            var DummyPlayer2 = dbContext.players.First(p => p.Nickname == DummyUserNickname2);
            var DummyPlayer3 = dbContext.players.First(p => p.Nickname == DummyUserNickname3);
            var DummyPlayer4 = dbContext.players.First(p => p.Nickname == DummyUserNickname4);
            var DummyPlayer5 = dbContext.players.First(p => p.Nickname == DummyUserNickname5);
            var Friendship1 = dbContext.friendships.First(f => f.Sender == DummyPlayer1.UserId && f.Receiver == DummyPlayer2.UserId);
            var Friendship2 = dbContext.friendships.First(f => f.Sender == DummyPlayer2.UserId && f.Receiver == DummyPlayer4.UserId);
            var Friendship3 = dbContext.friendships.First(f => f.Sender == DummyPlayer4.UserId && f.Receiver == DummyPlayer1.UserId);
            var Friendship4 = dbContext.friendships.First(f => f.Sender == DummyPlayer4.UserId && f.Receiver == DummyPlayer5.UserId);
            dbContext.friendships.Remove(Friendship1);
            dbContext.friendships.Remove(Friendship2);
            dbContext.friendships.Remove(Friendship3);
            dbContext.friendships.Remove(Friendship4);
            dbContext.players.Remove(DummyPlayer1);
            dbContext.players.Remove(DummyPlayer2);
            dbContext.players.Remove(DummyPlayer3);
            dbContext.players.Remove(DummyPlayer4);
            dbContext.players.Remove(DummyPlayer5);
            dbContext.SaveChanges();
        }

        [TestMethod]
        public void TestCheckPasswordSuccess() {
            var result = CheckPassword(InsecurePassword, InsecurePasswordHash);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCheckPasswordFailure() {
            var result = CheckPassword("wrongpassword", InsecurePasswordHash);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestGetPlayerDataSuccess() {
            var player = GetPlayerData(DummyUserNickname1);
            Assert.IsNotNull(player);
        }

        [TestMethod]
        public void TestGetPlayerDataFailure() {
            var player = GetPlayerData("NonExistentUser");
            Assert.IsNull(player);
        }

        [TestMethod]
        public void TestAuthenticatePlayerInvalidCredentials() {
            var authResult = AuthenticatePlayer("unexistinguser", "wrongpassword");
            var expectedResult = PlayerAuthResult.InvalidCredentials;
            Assert.AreEqual(expectedResult, authResult);
        }

        [TestMethod]
        public void TestAuthenticaPlayerIncorrectPassword() {
            var authResult = AuthenticatePlayer(DummyUserNickname1, "wrongpassword");
            var expectedResult = PlayerAuthResult.IncorrectPassword;
            Assert.AreEqual(expectedResult, authResult);
        }

        [TestMethod]
        public void TestAuthenticatePlayerNotVerified() {
            var authResult = AuthenticatePlayer(DummyUserNickname4, SecurePassword);
            var expectedResult = PlayerAuthResult.NotVerified;
            Assert.AreEqual(expectedResult, authResult);
        }

        [TestMethod]
        public void TestAuthenticatePlayerSuccess() {
            var authResult = AuthenticatePlayer(DummyUserNickname2, SecurePassword);
            var expectedResult = PlayerAuthResult.Success;
            Assert.AreEqual(expectedResult, authResult);
        }

        [TestMethod]
        public void TestRegisterPlayerSuccess() {
            var registerResult = RegisterPlayer(DummyUserNickname3, SecurePassword, Email);
            var expectedResult = PlayerResgisterResult.Success;
            Assert.AreEqual(expectedResult, registerResult);
        }

        [TestMethod]
        public void TestRegisterPlayerNicknameAlreadyExists() {
            var registerResult = RegisterPlayer(DummyUserNickname1, SecurePassword, Email);
            var expectedResult = PlayerResgisterResult.PlayerAlreadyExists;
            Assert.AreEqual(expectedResult, registerResult);
        }

        [TestMethod]
        public void TestRegisterInvalidInputs() {
            var registerResult = RegisterPlayer("", "", "");
            var expectedResult = PlayerResgisterResult.InvalidInputs;
            Assert.AreEqual(expectedResult, registerResult);
        }

        [TestMethod]
        public void TestVerifyPlayerSuccess() {
            var verifyResult = VerifyPlayer(DummyPlayer1.Nickname, SecurePassword, DummyPlayer1.VerificationCode);
            var expectedResult = PlayerVerificationResult.Success;
            Assert.AreEqual(expectedResult, verifyResult);
        }

        [TestMethod]
        public void TestVerifyPlayerAuthFailer() {
            var verifyResult = VerifyPlayer(DummyPlayer1.Nickname, "wrongpassword", DummyPlayer1.VerificationCode);
            var expectedResult = PlayerVerificationResult.AuthFailed;
            Assert.AreEqual(expectedResult, verifyResult);
        }

        [TestMethod]
        public void TestRequestFriendshipSuccess() {
            var requestResult = RequestFriendship(DummyUserNickname1, DummyUserNickname2);
            var expectedResult = PlayerFriendRequestResult.Success;
            Assert.AreEqual(expectedResult, requestResult);
        }

        [TestMethod]
        public void TestRequestFriendshipPending() {
            var requestResult = RequestFriendship(DummyUserNickname2, DummyUserNickname4);
            var expectedResult = PlayerFriendRequestResult.PendingRequest;
            Assert.AreEqual(expectedResult, requestResult);
        }

        public void TestRequestFriendshipAlreadyFriends() {
            var requestResult = RequestFriendship(DummyUserNickname4, DummyUserNickname5);
            var expectedResult = PlayerFriendRequestResult.AlreadyFriends;
            Assert.AreEqual(expectedResult, requestResult);
        }

        [TestMethod]
        public void TestAnswerFriendRequestAcceptSuccess() {
            var requestResult = AnswerFriendshipRequest(DummyUserNickname1, DummyUserNickname4, true);
            var expectedResult = PlayerFriendshipRequestAnswer.Success;
            Assert.AreEqual(expectedResult, requestResult);
        }

        [TestMethod]
        public void TestGetPlayerFriendsDataSuccess() {
            var friendsData = GetPlayerFriendsData(DummyUserNickname5);
            var expectedFriends = 1;
            Assert.AreEqual(expectedFriends, friendsData.Count);
        }
    }
}
