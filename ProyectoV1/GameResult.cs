using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1
{
    partial class GameResult
    {
        int idGameResult;
        int gameId;
        int playerId;
        int points;
        int position;

        public GameResult(int idGameResult, int gameId, int playerId, int points, int position)
        {
            this.idGameResult = idGameResult;
            this.gameId = gameId;
            this.playerId = playerId;
            this.points = points;
            this.position = position;
        }

        public int IdGameResult
        {
            get { return idGameResult; }
            set { idGameResult = value; }
        }

        public int GameId
        {
            get { return gameId; }
            set { gameId = value; }
        }

        public int PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
        }

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
