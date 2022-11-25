﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    public interface IPartyMananger {
        [OperationContract(IsOneWay = true)]
        void CreateParty(string sessionId);

        [OperationContract(IsOneWay = true)]
        void LeaveParty();

        [OperationContract(IsOneWay = true)]
        void StartGame();

        [OperationContract(IsOneWay = true)]
        void CancelGame();

        [OperationContract(IsOneWay = true)]
        void InvitePlayer(Player player);

        [OperationContract(IsOneWay = true)]
        void AcceptInvitation(Player player);

        [OperationContract(IsOneWay = true)]
        void DeclineInvitation(Player player);

        [OperationContract(IsOneWay = true)]
        void KickPlayer(Player player);

        [OperationContract(IsOneWay = true)]
        void TransferLeadership(Player player);
    }

    public interface IPartyManagerCallback {
        [OperationContract]
        void CreatePartyCallback(Party party);

        [OperationContract]
        void ReceiveInvitation(Player player);

        [OperationContract]
        void ReceiveInvitationDecline(Player player);

        [OperationContract]
        void ReceivePartyPlayerLeave(Player player);

        [OperationContract]
        void ReceivePartyPlayerJoin(Player player);

        [OperationContract]
        void ReceiveGameStart();

        [OperationContract]
        void ReceiveGameCancel();

        [OperationContract]
        void ReceivePartyKick(Player player);
        
        [OperationContract]
        void ReceivePartyLeaderTransfer(Player player);
    }

    [DataContract]
    public class Party {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public Player Leader { get; set; }

        [DataMember]
        public List<Player> Players { get; set; }

        public void PlayerLeaves(Player player) {
            Players.Remove(player);
            foreach(var p in Players) {
                p.PartyCallbackChannel.ReceivePartyPlayerLeave(player);
            }
            
            if(Leader == player) {
                Leader = Players[0];
                foreach(var p in Players) {
                    p.PartyCallbackChannel.ReceivePartyLeaderTransfer(Leader);
                }
            }
        }

        public Party() {
            Id = Guid.NewGuid().ToString();
        }
    }

    public partial class Player {
        [DataMember]
        public Party CurrentParty { get; set; }

        [IgnoreDataMember]
        public IPartyManagerCallback PartyCallbackChannel { get; set; }

        public void Dispose() {
            CurrentParty.PlayerLeaves(this);
        }
    }
}
