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
        void PlaceTile(int rackTileIndex, int x, int y);

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

        [OperationContract]
        void GameEnd(Party party);
    }
}
