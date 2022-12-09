using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using static Core.Game;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPartyGameCallback))]
    public interface IPartyGame {
        [OperationContract(IsOneWay = true)]
        void ConnectPartyGame(string playerSessionId);

        [OperationContract(IsOneWay = true)]
        void PlaceTile(Tile tile, int x, int y);

        [OperationContract(IsOneWay = true)]
        void EndTurn();

        [OperationContract(IsOneWay = true)]
        void PassTurn();
    }

    public interface IPartyGameCallback {
        [OperationContract]
        void UpdateBoard(BoardSlot[][] board);

        [OperationContract]
        void UpdatePlayerRack(Tile?[] rack);

        [OperationContract]
        void UpdatePlayerScore(Player player, int score);

        [OperationContract]
        void UpdatePlayerTurn(Player player);

        [OperationContract]
        void UpdateBagTilesLeft(int amount);

        [OperationContract]
        void SendInvalidTilePlacingError();
    }

    public partial class Party {
        [IgnoreDataMember]
        public Game Game { get; set; }

        [IgnoreDataMember]
        public int CurrentPlayerTurn { get; set; }

        [IgnoreDataMember]
        public int TimeLimitMins { get; set; }

        [IgnoreDataMember]
        public Timer Timer { get; set; }

        public Player NextTurn() {
            CurrentPlayerTurn = (CurrentPlayerTurn + 1) % Players.Count;
            return Players[CurrentPlayerTurn];
        }
    }

    public partial class Player {
        [IgnoreDataMember]
        public Tile?[] Rack { get; set; }

        [IgnoreDataMember]
        public int Score { get; set; }

        [IgnoreDataMember]
        public IPartyGameCallback PartyGameCallbackChannel { get; set; }

        public int UsedRackTiles() {
            int count = 0;
            foreach(var tile in Rack) {
                if(tile == null) {
                    count++;
                }
            }
            return count;
        }
    }
}
