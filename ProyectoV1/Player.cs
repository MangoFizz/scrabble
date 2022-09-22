using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1
{
    partial class Player
    {
        int idUsuario;
        String username;
        String password;

        public Player(int idUsuario, String username, String password)
        {
            this.idUsuario = idUsuario;
            this.username = username;
            this.password = password;
        }

        public int IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }

        public String Username
        {
            get { return username; }
            set { username = value; }
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
