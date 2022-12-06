using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPartyGameCallback))]
    public interface IPartyGame {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void ConnectPartyGame(string playerSessionId);
    }

    public interface IPartyGameCallback {
        [OperationContract(IsOneWay = true)]
        void UpdateBoard(Game.BoardSlot[,] board);
    }

    public partial class Party {
        public Game Game { get; set; }

        public int TimeLimitMins { get; set; }
    }

    public partial class Player {
        [IgnoreDataMember]
        public IPartyGameCallback PartyGameCallbackChannel { get; set; }
    }
}