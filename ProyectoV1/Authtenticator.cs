using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1
{
    internal class Authtenticator
    {
         public enum AuthLoginError
        {
            AUTH_LOGIN_ERROR_SUCCESS = 0,
            AUTH_LOGIN_ERROR_INVALID_CREDENTIALS,
            AUTH_LOGIN_ERROR_DATABASE_ERROR,
            AUTH_LOGIN_ERROR_UNKNOWN
        }

        public static AuthLoginError login(String username, String password)
        {
            Scrabble99Entities context = new Scrabble99Entities();
            var user = from player in context.players where player.username == username && player.password == password select player;
            if (user.get)
            {

            }
            return AuthLoginError.AUTH_LOGIN_ERROR_SUCCESS;
        }
    }
}
