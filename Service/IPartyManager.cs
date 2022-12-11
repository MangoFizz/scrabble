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

    public enum JoinPartyResult {
        Success,
        PartyNotFound,
        PartyIsFull,
        WhoAreYou
    }

    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    public interface IPartyManager {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void ConnectPartyManager(string sessionId);

        [OperationContract]
        Party CreateParty();

        [OperationContract(IsOneWay = true)]
        void LeaveParty();

        [OperationContract(IsOneWay = true)]
        void StartGame(Game.SupportedLanguage language, int timeLimitMins);

        [OperationContract(IsOneWay = true)]
        void JoinParty(string inviteCode);

        [OperationContract(IsOneWay = true)]
        void InviteFriend(Player player);

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
        void JoinPartyCallback(JoinPartyResult result, Party party);

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
}
