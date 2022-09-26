using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DataAccess
{
    public class Authtenticator
    {
        public enum AuthLoginResult
        {
            SUCCESS = 0,
            INVALID_CREDENTIALS,
            INCORRECT_PASSWORD,
            DATABASE_ERROR,
            UNKNOWN
        }

        public enum AuthRegisterResult
        {
            SUCCESS = 0,
            USER_ALREADY_EXISTS,
            DATABASE_ERROR,
            UNKNOWN
        }

        private static String hashPassword(String password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedPassword = sha256.ComputeHash(passwordBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                builder.Append(hashedPassword[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static bool checkPassword(String password, String hashedPassword)
        {
            return hashPassword(password).Equals(hashedPassword);
        }

        public static AuthLoginResult login(String username, String password)
        {
            try
            {
                Scrabble99Entities context = new Scrabble99Entities();
                var queryResult = from player in context.players where player.username == username select player;
                if (queryResult.Count() > 0)
                {
                    var user = queryResult.First();
                    if (checkPassword(password, user.password))
                    {
                        return AuthLoginResult.SUCCESS;
                    }
                    return AuthLoginResult.INCORRECT_PASSWORD;
                }
                return AuthLoginResult.INVALID_CREDENTIALS;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return AuthLoginResult.DATABASE_ERROR;
            }
        }
        
        public static AuthRegisterResult register(String username, String password)
        {
            try
            {
                Scrabble99Entities context = new Scrabble99Entities();
                var queryResult = from player in context.players where player.username == username select player;
                if (queryResult.Count() == 0)
                {
                    players player = new players();
                    player.username = username;
                    player.password = hashPassword(password);
                    context.players.Add(player);
                    context.SaveChanges();

                    return AuthRegisterResult.SUCCESS;
                }
                return AuthRegisterResult.USER_ALREADY_EXISTS;
            }
            catch (Exception ex)
            {
                return AuthRegisterResult.DATABASE_ERROR;
            }
        }
    }
}
