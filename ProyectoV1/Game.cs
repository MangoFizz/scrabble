using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1
{
    partial class Game
    {
        int idGame;
        int duration;
        DateTime date;
        int idWinner;

        public Game(int idGame, int duration, DateTime date, int idWinner)
        {
            this.idGame = idGame;
            this.duration = duration;
            this.date = date;
            this.idWinner = idWinner;
        }

        public int IdGame
        {
            get { return idGame; }
            set { idGame = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public int IdWinner
        {
            get { return idWinner; }
            set { idWinner = value; }
        }
    }
}
