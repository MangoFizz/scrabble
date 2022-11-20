﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Core;

using static Core.PlayerManager;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPlayerManagerCallback), SessionMode = SessionMode.Required)]
    public interface IPlayerManager {
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void Login(string nickname, string password);

        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void RegisterPlayer(string nickname, string password, string email);
        
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
        void GetFriendListResponseHandler(Player[] friends);

        [OperationContract]
        void GetFriendRequestsResponseHandler(Player[] friendRequests);

        [OperationContract]
        void ReceiveFriendRequest(Player player);

        [OperationContract]
        void ReceiveFriendAdd(Player player);
    }

    /// <summary>
    /// Partial class for player data contract.
    /// </summary>
    [DataContract]
    public partial class Player {
        [DataMember]
        public string Nickname { get; set; }

        /// <summary>
        /// Avatar index for player.
        /// </summary>
        [DataMember]
        public int Avatar { get; set; }

        /// <summary>
        /// Only avaiable for the client logged user. Other player contracts will have this field as null.
        /// </summary>
        [DataMember]
        public List<Player> Friends { get; set; }

        /// <summary>
        /// Only avaiable for the client logged user. Other player contracts will have this field as null.
        /// </summary>
        [DataMember]
        public List<Player> FriendRequests { get; set; }

        [IgnoreDataMember]
        public IPlayerManagerCallback PlayerManagerCallbackChannel { get; set; }

        public Player(DataAccess.Player playerData) {
            Nickname = playerData.Nickname;
            Avatar = playerData.Avatar;
            Friends = null;
            FriendRequests = null;
        }
    }
}
