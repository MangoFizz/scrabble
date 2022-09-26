using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System;

namespace Tests
{
    [TestClass]
    public class AuthtenticatorTests
    {
        [TestMethod]
        public void testHashPassword()
        {
            PrivateType authtenticator = new PrivateType(typeof(Authtenticator));
            String password = "password";
            String expectedHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";
            String hashedPassword = authtenticator.InvokeStatic("hashPassword", password) as String;
            Assert.AreEqual(expectedHash, hashedPassword);
        }

        [TestMethod]
        public void testCheckPassword()
        {
            PrivateType authtenticator = new PrivateType(typeof(Authtenticator));
            String password = "password";
            String hashedPassword = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";
            bool result = (bool)authtenticator.InvokeStatic("checkPassword", password, hashedPassword);
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void TestLoginSuccess()
        {
            String username = "testUser";
            String password = "password";
            Authtenticator.AuthLoginResult result = Authtenticator.login(username, password);
            Assert.AreEqual(Authtenticator.AuthLoginResult.SUCCESS, result);
        }

        [TestMethod]
        public void TestLoginInvalidCredentials()
        {
            String username = "wrongUser";
            String password = "testPassword";
            Authtenticator.AuthLoginResult result = Authtenticator.login(username, password);
            Assert.AreEqual(Authtenticator.AuthLoginResult.INVALID_CREDENTIALS, result);
        }

        [TestMethod]
        public void TestLoginIncorrectPassword()
        {
            String username = "testUser";
            String password = "wrongPassword";
            Authtenticator.AuthLoginResult result = Authtenticator.login(username, password);
            Assert.AreEqual(Authtenticator.AuthLoginResult.INCORRECT_PASSWORD, result);
        }
    }
}
