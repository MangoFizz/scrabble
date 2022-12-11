using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Core;

using static Core.PlayerManager;

namespace Service {
    public enum DisconnectionReason {
        DuplicatePlayerSession,
        ServerShutdown,
        ServerInternalError
    }
    
    [ServiceContract(CallbackContract = typeof(IPlayerManagerCallback), SessionMode = SessionMode.Required)]
    public interface IPlayerManager {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Login(string nickname, string password);

        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void LoginAsGuest();

        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void RegisterPlayer(string nickname, string password, string email);

        [OperationContract(IsOneWay = true)]
        void ResendVerificationCode(string nickname, string password);

        [OperationContract(IsOneWay = true)]
        void VerifyPlayer(string nickname, string password, string code);
        
        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Logout();

        [OperationContract(IsOneWay = true)]
        void GetFriendList();

        [OperationContract(IsOneWay = true)]
        void GetFriendRequests();

        [OperationContract(IsOneWay = true)]
        void SendFriendRequest(string nickname);

        [OperationContract(IsOneWay = true)]
        void AcceptFriendRequest(string nickname);

        [OperationContract(IsOneWay = true)]
        void DeclineFriendRequest(string nickname);

        [OperationContract(IsOneWay = true)]
        void UpdatePlayerAvatar(short newAvatarId);
    }

    [ServiceContract]
    public interface IPlayerManagerCallback {
        [OperationContract(IsOneWay = true)]
        void LoginResponseHandler(PlayerAuthResult loginResult, Player player, string sessionId);

        [OperationContract(IsOneWay = true)]
        void VerificationResponseHandler(PlayerVerificationResult verificationResult);

        [OperationContract(IsOneWay = true)]
        void RegisterPlayerResponseHandler(PlayerResgisterResult registrationResult);

        [OperationContract(IsOneWay = true)]
        void SendFriendRequestResponseHandler(PlayerFriendRequestResult result);

        [OperationContract(IsOneWay = true)]
        void GetFriendListResponseHandler(Player[] friends);

        [OperationContract(IsOneWay = true)]
        void GetFriendRequestsResponseHandler(Player[] friendRequests);

        [OperationContract(IsOneWay = true)]
        void ReceiveFriendRequest(Player player);

        [OperationContract(IsOneWay = true)]
        void ReceiveFriendAdd(Player player);

        [OperationContract]
        void UpdateFriendStatus(Player friend, Player.StatusType status);

        [OperationContract]
        void UpdatePlayerAvatarCallback(short avatarId);

        [OperationContract]
        void Disconnect(DisconnectionReason reason);
    }
}
