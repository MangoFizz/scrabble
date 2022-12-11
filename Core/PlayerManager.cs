using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data.Common;
using DataAccess;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace Core {
    public class PlayerManager {
        public enum PlayerAuthResult {
            Success = 0,
            NotVerified,
            InvalidCredentials,
            IncorrectPassword,
            DatabaseError
        }

        public enum PlayerResgisterResult {
            Success = 0,
            PlayerAlreadyExists,
            InvalidInputs,
            DatabaseError
        }

        public enum PlayerResendCodeResult {
            Success = 0,
            PlayerDoesNotExists,
            UserAlreadyVerified,
            DatabaseError,
            InternalError
        }

        public enum PlayerVerificationResult {
            Success = 0,
            AuthFailed,
            InvalidCode,
            DatabaseError
        }

        public enum PlayerUnregisterResult {
            Success = 0,
            AuthFailed,
            PlayerDoesNotExists,
            DatabaseError
        }

        public enum PlayerFriendshipStatus : short {
            Pending = 0,
            Accepted,
        }

        public enum PlayerFriendRequestResult {
            Success = 0,
            SelfRequest,
            AlreadyFriends,
            PendingRequest,
            SenderPlayerDoesNotExists,
            ReceiverPlayerDoesNotExists,
            DatabaseError
        }

        public enum PlayerFriendshipRequestAnswer {
            Success = 0,
            SenderPlayerDoesNotExists,
            ReceiverPlayerDoesNotExists,
            FriendshipRequestDoesNotExists,
            DatabaseError
        }

        private static string HashPassword(string password) {
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < hashBytes.Length; i++) {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        private static bool CheckPassword(string password, string hashedPassword) {
            return HashPassword(password).Equals(hashedPassword);
        }

        private static bool ValidateRegisterInputs(string nickname, string password, string email) {
            var validCharactersRegex = new Regex("^[a-zA-Z0-9 ]*$");
            var passwordValidCharactersRegex = new Regex("^[a-zA-Z0-9!\" \"# $% & '() * +, -. / :; <=>? @ \\ [\\] \\ ^ _` {|} ~]*$");
            var containNumbersRegex = new Regex(@"(?=.*\d)");
            var containLowercaseRegex = new Regex(@"(?=.*[a-z])");
            var containUppercaseRegex = new Regex(@"(?=.*[A-Z])");

            bool isInputValid = true;

            if(nickname.Length == 0 || nickname.Length > 12 || nickname.Length < 4 || !validCharactersRegex.IsMatch(nickname)) {
                isInputValid = false;
            }

            if(email.Length == 0 || email.Length > 255) {
                isInputValid = false;
            }
            else {
                try {
                    new System.Net.Mail.MailAddress(email);
                }
                catch(FormatException) {
                    isInputValid = false;
                }
            }

            if(password.Length == 0 || password.Length > 255 || password.Length < 8 || !passwordValidCharactersRegex.IsMatch(password)) {
                isInputValid = false;
            }

            if(!containLowercaseRegex.IsMatch(password) || !containUppercaseRegex.IsMatch(password) || !containNumbersRegex.IsMatch(password)) {
                isInputValid = false;
            }

            return isInputValid;
        }

        private static string GenerateVerificationCode() {
            var random = new Random();
            var verificationCode = random.Next(0x1000, 0xFFFF).ToString("x2");
            return verificationCode.ToUpper();
        }
        
        private static void SendVerificationCodeEmail(Player player) {
            var senderEmail = new MailAddress(ConfigurationManager.AppSettings["emailAddress"], "Scrabble game");
            var playerEmail = new MailAddress(player.Email, $"To {player.Nickname}");
            string appPassword = ConfigurationManager.AppSettings["emailAppPassword"];
            const string subject = "Scrabble account verification code";
            string body = $"Your verification code is \"{player.VerificationCode}\".";

            var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, appPassword)
            };
            try {
                using(var message = new MailMessage(senderEmail, playerEmail)) {
                    message.Subject = subject;
                    message.Body = body;
                    smtp.Send(message);
                }
            }
            catch {
                throw;
            }
        }

        public static Player GetPlayerData(string nickname) {
            ScrabbleEntities context = new ScrabbleEntities();
            return context.players.First(p => p.Nickname == nickname);
        } 

        public static PlayerAuthResult AuthenticatePlayer(string nickname, string password) {
            try {
                ScrabbleEntities context = new ScrabbleEntities();
                var queryResult = from player in context.players where player.Nickname == nickname select player;
                if(queryResult.Count() > 0) {
                    var user = queryResult.First();
                    if(CheckPassword(password, user.Password)) {
                        if(!user.Verified) {
                            return PlayerAuthResult.NotVerified;
                        }
                        return PlayerAuthResult.Success;
                    }
                    return PlayerAuthResult.IncorrectPassword;
                }
                return PlayerAuthResult.InvalidCredentials;
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerAuthResult.DatabaseError;
            }
        }

        public static PlayerResgisterResult RegisterPlayer(string nickname, string password, string email) {
            if(!ValidateRegisterInputs(nickname, password, email)) {
                return PlayerResgisterResult.InvalidInputs;
            }

            try {
                ScrabbleEntities context = new ScrabbleEntities();
                
                if(context.players.Any(p => p.Nickname == nickname)) {
                    return PlayerResgisterResult.PlayerAlreadyExists;
                }
                
                Player player = new Player() {
                    Nickname = nickname,
                    Password = HashPassword(password),
                    Email = email,
                    Registered = DateTime.Now,
                    VerificationCode = GenerateVerificationCode()
                };
                context.players.Add(player);
                context.SaveChanges();

                SendVerificationCodeEmail(player);

                return PlayerResgisterResult.Success;
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerResgisterResult.DatabaseError;
            }
        }

        public static PlayerResendCodeResult ResendVerificationCode(string nickname) {
            try {
                ScrabbleEntities context = new ScrabbleEntities();
                var queryResult = from player in context.players where player.Nickname == nickname select player;
                if(queryResult.Count() > 0) {
                    var player = queryResult.First();
                    try {
                        SendVerificationCodeEmail(player);
                        return PlayerResendCodeResult.Success;
                    }
                    catch(SmtpException ex) {
                        Console.WriteLine(ex.Message);
                        return PlayerResendCodeResult.InternalError;
                    }
                }
                return PlayerResendCodeResult.PlayerDoesNotExists;
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerResendCodeResult.DatabaseError;
            }
        }

        public static PlayerVerificationResult VerifyPlayer(string nickname, string password, string code) {
            if(AuthenticatePlayer(nickname, password) != PlayerAuthResult.NotVerified) {
                return PlayerVerificationResult.AuthFailed;
            }
            try {
                ScrabbleEntities context = new ScrabbleEntities();
                var queryResult = from player in context.players where player.Nickname == nickname select player;
                if(queryResult.Count() > 0) {
                    var user = queryResult.First();
                    if(user.VerificationCode == code.ToUpper()) {
                        user.Verified = true;
                        context.SaveChanges();
                        return PlayerVerificationResult.Success;
                    }
                    return PlayerVerificationResult.InvalidCode;
                }
                return PlayerVerificationResult.AuthFailed;
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerVerificationResult.DatabaseError;
            }
        }
        
        public static PlayerUnregisterResult UnregisterPlayer(string nickname, string password) {
            try {
                ScrabbleEntities context = new ScrabbleEntities();
                var queryResult = from player in context.players where player.Nickname == nickname select player;
                if(queryResult.Count() > 0) {
                    if(AuthenticatePlayer(nickname, password) != PlayerAuthResult.Success) {
                        return PlayerUnregisterResult.AuthFailed;
                    }
                    var player = queryResult.First();
                    context.players.Remove(player);
                    context.SaveChanges();
                    return PlayerUnregisterResult.Success;
                }
                else {
                    return PlayerUnregisterResult.PlayerDoesNotExists;
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerUnregisterResult.DatabaseError;
            }
        }

        public static PlayerFriendRequestResult RequestFriendship(string playerNickname, string receiverPlayerNickname) {
            if(playerNickname == receiverPlayerNickname) {
                return PlayerFriendRequestResult.SelfRequest;
            }

            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    if(player == null) {
                        return PlayerFriendRequestResult.SenderPlayerDoesNotExists;
                    }
                    
                    var receiverPlayer = context.players.FirstOrDefault(p => p.Nickname == receiverPlayerNickname);
                    if(receiverPlayer == null) {
                        return PlayerFriendRequestResult.ReceiverPlayerDoesNotExists;
                    }

                    var friendship = context.friendships.FirstOrDefault(f => (f.Sender == player.UserId && f.Receiver == receiverPlayer.UserId) || (f.Sender == receiverPlayer.UserId && f.Receiver == player.UserId));
                    if(friendship != null) {
                        if(friendship.Status == (short)PlayerFriendshipStatus.Pending) {
                            return PlayerFriendRequestResult.PendingRequest;
                        }
                        else {
                            return PlayerFriendRequestResult.AlreadyFriends;
                        }
                        
                    }

                    friendship = new Friendship() {
                        Sender = player.UserId,
                        Receiver = receiverPlayer.UserId,
                        Status = (short)PlayerFriendshipStatus.Pending
                    };
                    context.friendships.Add(friendship);
                    context.SaveChanges();

                    return PlayerFriendRequestResult.Success;
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerFriendRequestResult.DatabaseError;
            }
        }

        public static PlayerFriendshipRequestAnswer AnswerFriendshipRequest(string playerNickname, string senderPlayerNickname, bool accept) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    if(player == null) {
                        return PlayerFriendshipRequestAnswer.ReceiverPlayerDoesNotExists;
                    }

                    var senderPlayer = context.players.FirstOrDefault(p => p.Nickname == senderPlayerNickname);
                    if(senderPlayer == null) {
                        return PlayerFriendshipRequestAnswer.SenderPlayerDoesNotExists;
                    }

                    var friendship = context.friendships.FirstOrDefault(f => f.Sender == senderPlayer.UserId && f.Receiver == player.UserId);
                    if(friendship == null) {
                        return PlayerFriendshipRequestAnswer.FriendshipRequestDoesNotExists;
                    }

                    if(accept) {
                        friendship.Status = (short)PlayerFriendshipStatus.Accepted;
                    }
                    else {
                        context.friendships.Remove(friendship);
                    }

                    context.SaveChanges();
                    
                    return PlayerFriendshipRequestAnswer.Success;
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerFriendshipRequestAnswer.DatabaseError;
            }
        }

        public static List<Player> GetPlayerFriendsData(string playerNickname) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    var friendships = context.friendships.Where(f => (f.Sender == player.UserId || f.Receiver == player.UserId) && f.Status == (short)PlayerFriendshipStatus.Accepted).ToList();
                    var friends = new List<Player>();
                    foreach(var friendship in friendships) {
                        if(friendship.Sender == player.UserId) {
                            friends.Add(context.players.FirstOrDefault(p => p.UserId == friendship.Receiver));
                        }
                        else {
                            friends.Add(context.players.FirstOrDefault(p => p.UserId == friendship.Sender));
                        }
                    }
                    return friends;
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static List<Player> GetPrendingFriendRequest(string playerNickname) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    var friendships = context.friendships.Where(f => f.Receiver == player.UserId && f.Status == (short)PlayerFriendshipStatus.Pending).ToList();
                    var friends = new List<Player>();
                    foreach(var friendship in friendships) {
                        friends.Add(context.players.FirstOrDefault(p => p.UserId == friendship.Sender));
                    }
                    return friends;
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void UpdatePlayerAvatar(string playerNickname, short avatarId) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    player.Avatar = avatarId;
                    context.SaveChanges();
                }
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
