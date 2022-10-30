using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Core;

using static Core.PlayerManager;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPlayerManagerCallback))]
    public interface IPlayerManager {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Login(string nickname, string password);

        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void RegisterPlayer(string nickname, string password, string email);
        
        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Logout();

        [OperationContract(IsOneWay = true)]
        void SendFriendRequest(string nickname);

        [OperationContract(IsOneWay = true)]
        void AcceptFriendRequest(string nickname);

        [OperationContract(IsOneWay = true)]
        void DeclineFriendRequest(string nickname);
    }

    [ServiceContract]
    public interface IPlayerManagerCallback {
        [OperationContract]
        void LoginResponseHandler(PlayerAuthResult loginResult, Player player);

        [OperationContract]
        void RegisterPlayerResponseHandler(PlayerResgisterResult registrationResult);

        [OperationContract]
        void SendFriendRequestResponseHandler(PlayerFriendRequestResult result);

        [OperationContract]
        void ReceiveFriendRequest(Player player);

        [OperationContract]
        void ReceiveFriendAdd(Player player);
    }

    [DataContract]
    public partial class Player {
        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public int Avatar { get; set; }

        [DataMember]
        public List<Player> Friends { get; set; }

        [DataMember]
        public List<Player> FriendRequest { get; set; }

        [IgnoreDataMember]
        public IPlayerManagerCallback PlayerManagerCallbackChannel { get; set; }
    }
}
