using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data.Common;
using DataAccess;
using System.Collections.Generic;

namespace Core {
    public class PlayerManager {
        public enum PlayerAuthResult {
            Success = 0,
            InvalidCredentials,
            IncorrectPassword,
            DatabaseError
        }

        public enum PlayerResgisterResult {
            Success = 0,
            PlayerAlreadyExists,
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

            // Convert hash bytes to string in hex format
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < hashBytes.Length; i++) {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        private static bool CheckPassword(string password, string hashedPassword) {
            return HashPassword(password).Equals(hashedPassword);
        }

        public static Player GetPlayerData(string nickname) {
            Scrabble99Entities context = new Scrabble99Entities();
            return context.players.First(p => p.Nickname == nickname);
        } 

        public static PlayerAuthResult AuthenticatePlayer(string nickname, string password) {
            try {
                Scrabble99Entities context = new Scrabble99Entities();
                var queryResult = from player in context.players where player.Nickname == nickname select player;
                if(queryResult.Count() > 0) {
                    var user = queryResult.First();
                    if(CheckPassword(password, user.Password)) {
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
            try {
                Scrabble99Entities context = new Scrabble99Entities();
                
                if(context.players.Any(p => p.Nickname == nickname)) {
                    return PlayerResgisterResult.PlayerAlreadyExists;
                }
                
                Player player = new Player() {
                    Nickname = nickname,
                    Password = HashPassword(password),
                    Email = email
                };
                context.players.Add(player);
                context.SaveChanges();

                return PlayerResgisterResult.Success;
            }
            catch(DbException ex) {
                Console.WriteLine(ex.Message);
                return PlayerResgisterResult.DatabaseError;
            }
        }

        public static PlayerUnregisterResult UnregisterPlayer(string nickname, string password) {
            try {
                Scrabble99Entities context = new Scrabble99Entities();
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
            try {
                using(Scrabble99Entities context = new Scrabble99Entities()) {
                    var player = context.players.FirstOrDefault(p => p.Nickname == playerNickname);
                    if(player == null) {
                        return PlayerFriendRequestResult.SenderPlayerDoesNotExists;
                    }
                    
                    var receiverPlayer = context.players.FirstOrDefault(p => p.Nickname == receiverPlayerNickname);
                    if(receiverPlayer == null) {
                        return PlayerFriendRequestResult.ReceiverPlayerDoesNotExists;
                    }

                    var friendship = new Friendship() {
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
                using(Scrabble99Entities context = new Scrabble99Entities()) {
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
                using(Scrabble99Entities context = new Scrabble99Entities()) {
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
                using(Scrabble99Entities context = new Scrabble99Entities()) {
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
    }
}
