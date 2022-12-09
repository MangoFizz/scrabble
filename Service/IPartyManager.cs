using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Service {
    public enum GameStartResult {
        Success,
        NotEnoughPlayers
    }

    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    public interface IPartyManager {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Subscribe(string sessionId);

        [OperationContract(IsOneWay = true)]
        void CreateParty();

        [OperationContract(IsOneWay = true)]
        void LeaveParty();

        [OperationContract(IsOneWay = true)]
        void StartGame(Game.SupportedLanguage language, int timeLimitMins);

        [OperationContract(IsOneWay = true)]
        void InvitePlayer(Player player);

        [OperationContract(IsOneWay = true)]
        void AcceptInvitation(Player player);

        [OperationContract(IsOneWay = true)]
        void KickPlayer(Player player);

        [OperationContract(IsOneWay = true)]
        void TransferLeadership(Player player);

        [OperationContract(IsOneWay = true)]
        void UpdateTimeLimitSetting(int time);

        [OperationContract(IsOneWay = true)]
        void UpdateLanguageSetting(Game.SupportedLanguage language);
    }

    public interface IPartyManagerCallback {
        [OperationContract]
        void CreatePartyCallback(Party party);

        [OperationContract]
        void ReceiveInvitation(Player player, string partyId);

        [OperationContract]
        void AcceptInvitationCallback(Party party);

        [OperationContract]
        void ReceiveInvitationDecline(Player player);

        [OperationContract]
        void ReceivePartyPlayerLeave(Player player);

        [OperationContract]
        void ReceivePartyPlayerJoin(Player player);

        [OperationContract]
        void StartGameCallback(GameStartResult result);

        [OperationContract]
        void ReceiveGameStart();

        [OperationContract]
        void ReceivePartyKick(Player player);
        
        [OperationContract]
        void ReceivePartyLeaderTransfer(Player player);

        [OperationContract]
        void ReceivePartyTimeLimitUpdate(int time);

        [OperationContract]
        void ReceivePartyLanguageUpdate(Game.SupportedLanguage language);
    }

    [DataContract]
    public partial class Party {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public Player Leader { get; set; }

        [DataMember]
        public List<Player> Players { get; set; }

        public void PlayerLeaves(Player player) {
            Players.Remove(player);
            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
            }
            
            if(Leader == player) {
                Leader = Players[0];
                foreach(var p in Players) {
                    p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(Leader);
                }
            }
        }

        public Party() {
            Players = new List<Player>();
            Id = Guid.NewGuid().ToString();
        }
    }

    public partial class Player {
        [IgnoreDataMember]
        public Party CurrentParty { get; set; }

        [IgnoreDataMember]
        public IPartyManagerCallback PartyManagerCallbackChannel { get; set; }

        public void Dispose() {
            CurrentParty.PlayerLeaves(this);
        }
    }
}
