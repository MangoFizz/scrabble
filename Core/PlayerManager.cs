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
using System.Diagnostics;

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

        public static string HashPassword(string password) {
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < hashBytes.Length; i++) {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static bool CheckPassword(string password, string hashedPassword) {
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

            System.Net.Mail.MailAddress emailAddress;
            if(email.Length == 0 || email.Length > 255) {
                isInputValid = false;
            }
            else {
                try {
                    emailAddress = new System.Net.Mail.MailAddress(email);
                }
                catch(FormatException ex) {
                    Log.Warning($"Invalid email address: {email}. Email format error ({ex.GetType().Name})");
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

        public static string GenerateVerificationCode() {
            var verificationCode = Core.Random.Next(0x1000, 0xFFFF).ToString("x2");
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
                    Log.Info($"Sent verification code email to {player.Nickname}.");
                }
            }
            catch(Exception e) {
                if(e is SmtpException || e is InvalidOperationException) {
                    Log.Error($"Failed to send verification code email to {player.Nickname}. {e.GetType().Name}: {e.Message}");
                }
                else {
                    throw;
                }
            }
        }

        public static Player GetPlayerData(string nickname) {
            ScrabbleEntities context = new ScrabbleEntities();
            return context.players.FirstOrDefault(p => p.Nickname == nickname);
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
                        Log.Info($"Authenticated player {nickname}.");
                        return PlayerAuthResult.Success;
                    }
                    return PlayerAuthResult.IncorrectPassword;
                }
                return PlayerAuthResult.InvalidCredentials;
            }
            catch(DbException ex) {
                Log.Error($"Failed to authenticate player {nickname}. Database error ({ex.GetType().Name})");
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

                Log.Info($"Registered player {nickname}.");
                return PlayerResgisterResult.Success;
            }
            catch(DbException ex) {
                Log.Error($"Failed to register player {nickname}. Database error ({ex.GetType().Name})");
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
                Log.Error($"Failed to resend verification email to player {nickname}. Database error ({ex.GetType().Name})");
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
                        Log.Info($"Verified player {nickname}.");
                        return PlayerVerificationResult.Success;
                    }
                    return PlayerVerificationResult.InvalidCode;
                }
                return PlayerVerificationResult.AuthFailed;
            }
            catch(DbException ex) {
                Log.Error($"Failed to verify player {nickname}. Database error ({ex.GetType().Name})");
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
                    Log.Info($"Player {nickname} has been unregistered.");
                    return PlayerUnregisterResult.Success;
                }
                else {
                    return PlayerUnregisterResult.PlayerDoesNotExists;
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to unregister player {nickname}. Database error ({ex.GetType().Name})");
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

                    var friendship = context.friendships.FirstOrDefault(f => (f.Sender == player.PlayerId && f.Receiver == receiverPlayer.PlayerId) || (f.Sender == receiverPlayer.PlayerId && f.Receiver == player.PlayerId));
                    if(friendship != null) {
                        if(friendship.Status == (short)PlayerFriendshipStatus.Pending) {
                            return PlayerFriendRequestResult.PendingRequest;
                        }
                        else {
                            return PlayerFriendRequestResult.AlreadyFriends;
                        }
                        
                    }

                    friendship = new Friendship() {
                        Sender = player.PlayerId,
                        Receiver = receiverPlayer.PlayerId,
                        Status = (short)PlayerFriendshipStatus.Pending
                    };
                    context.friendships.Add(friendship);
                    context.SaveChanges();

                    return PlayerFriendRequestResult.Success;
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to send friend request to player {receiverPlayerNickname} from player {playerNickname}. Database error ({ex.GetType().Name})");
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

                    var friendship = context.friendships.FirstOrDefault(f => f.Sender == senderPlayer.PlayerId && f.Receiver == player.PlayerId);
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
                    var friendships = context.friendships.Where(f => (f.Sender == player.PlayerId || f.Receiver == player.PlayerId) && f.Status == (short)PlayerFriendshipStatus.Accepted).ToList();
                    var friends = new List<Player>();
                    foreach(var friendship in friendships) {
                        if(friendship.Sender == player.PlayerId) {
                            friends.Add(context.players.FirstOrDefault(p => p.PlayerId == friendship.Receiver));
                        }
                        else {
                            friends.Add(context.players.FirstOrDefault(p => p.PlayerId == friendship.Sender));
                        }
                    }
                    Log.Info($"Fetched {friends.Count} friends for {playerNickname}.");
                    return friends; 

                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to fetch friend list for player {playerNickname}. Database error ({ex.GetType().Name})");
                return null;
            }
        }

        public static List<Player> GetPrendingFriendRequest(string playerNickname) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    var friendships = context.friendships.Where(f => f.Receiver == player.PlayerId && f.Status == (short)PlayerFriendshipStatus.Pending).ToList();
                    var friends = new List<Player>();
                    foreach(var friendship in friendships) {
                        friends.Add(context.players.FirstOrDefault(p => p.PlayerId == friendship.Sender));
                    }
                    Log.Info($"Fetched {friends.Count} friend requests for {playerNickname}.");
                    return friends;
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to fetch friend requests for player {playerNickname}. Database error ({ex.GetType().Name})");
                return null;
            }
        }

        public static void UpdatePlayerAvatar(string playerNickname, short avatarId) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    if(player != null) {
                        player.Avatar = avatarId;
                        Log.Info($"Updated avatar for {playerNickname}.");
                        context.SaveChanges();
                    }
                    else {
                        Log.Error($"Failed to update avatar for {playerNickname}. Player does not exists.");
                    }
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to update avatar for player {playerNickname}. Database error ({ex.GetType().Name})");
            }
        }

        public static int SaveGame(string winnerNickname) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var winner = GetPlayerData(winnerNickname);
                    if(winner != null) {
                        var game = new DataAccess.Game() {
                            WinnerId = winner.PlayerId,
                            Date = DateTime.Now
                        };
                        context.games.Add(game);
                        context.SaveChanges();
                        Log.Info($"Saved game for {winnerNickname}.");
                        return game.GameId;
                    }
                    else {
                        Log.Error($"Failed to save game. Player {winnerNickname} does not exists.");
                    }
                    
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to save game for player {winnerNickname}. Database error ({ex.GetType().Name})");
            }
            return -1;
        }

        public static void SaveGameResult(string playerNickname, int gameId, int score, int placement) {
            try {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    var player = GetPlayerData(playerNickname);
                    if(player != null) {
                        var gameResult = new GameResult() {
                            GameId = gameId,
                            PlayerId = player.PlayerId,
                            Score = score,
                            Placement = placement
                        };
                        context.gameResults.Add(gameResult);
                        context.SaveChanges();
                        Log.Info($"Saved game result for {playerNickname}.");
                    }
                    else {
                        Log.Error($"Failed to save game result. Player {playerNickname} does not exists.");
                    }
                }
            }
            catch(DbException ex) {
                Log.Error($"Failed to save game result for player {playerNickname}. Database error ({ex.GetType().Name})");
            }
        }
    }
}
