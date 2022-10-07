using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data.Common;

namespace DataAccess {
    public class Authenticator {
        public enum UserAuthResult {
            Success = 0,
            InvalidCredentials,
            IncorrectPassword,
            DatabaseError
        }

        public enum UserResgisterResult {
            Success = 0,
            UserAlreadyExists,
            DatabaseError
        }

        public enum UserUnregisterResult {
            Success = 0,
            AuthFailed,
            UserDoesNotExists,
            DatabaseError
        }

        private static String hashPassword(String password) {
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

        private static bool checkPassword(String password, String hashedPassword) {
            return hashPassword(password).Equals(hashedPassword);
        }

        public static UserAuthResult validateUser(String username, String password) {
            try {
                Scrabble99Entities context = new Scrabble99Entities();
                var queryResult = from player in context.players where player.username == username select player;
                if(queryResult.Count() > 0) {
                    var user = queryResult.First();
                    if(checkPassword(password, user.password)) {
                        return UserAuthResult.Success;
                    }
                    return UserAuthResult.IncorrectPassword;
                }
                return UserAuthResult.InvalidCredentials;
            }
            catch(DbException ex) {
                return UserAuthResult.DatabaseError;
            }
        }

        public static UserResgisterResult registerUser(String username, String password) {
            try {
                Scrabble99Entities context = new Scrabble99Entities();
                
                if(context.players.Any(p => p.username == username)) {
                    return UserResgisterResult.UserAlreadyExists;
                }
                
                players player = new players();
                player.username = username;
                player.password = hashPassword(password);
                context.players.Add(player);
                context.SaveChanges();

                return UserResgisterResult.Success;
            }
            catch(DbException ex) {
                return UserResgisterResult.DatabaseError;
            }
        }

        public static UserUnregisterResult unregisterUser(String username, String password) {
            try {
                Scrabble99Entities context = new Scrabble99Entities();
                var queryResult = from player in context.players where player.username == username select player;
                if(queryResult.Count() > 0) {
                    if(validateUser(username, password) != UserAuthResult.Success) {
                        return UserUnregisterResult.AuthFailed;
                    }
                    var player = queryResult.First();
                    context.players.Remove(player);
                    context.SaveChanges();
                    return UserUnregisterResult.Success;
                }
                else {
                    return UserUnregisterResult.UserDoesNotExists;
                }
            }
            catch(DbException ex) {
                return UserUnregisterResult.DatabaseError;
            }
        }

    }
}
