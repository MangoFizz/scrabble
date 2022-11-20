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
        [OperationContract(IsOneWay = true)]
        void LoginResponseHandler(PlayerAuthResult loginResult, Player player);

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

        [OperationContract(IsOneWay = true)]
        void FriendConnect(Player player);

        [OperationContract(IsOneWay = true)]
        void FriendDisconnect(Player player);
    }

    public enum PlayerStatus {
        Offline,
        Online,
        InGame
    }

    /// <summary>
    /// Partial class for player data contract.
    /// </summary>
    [DataContract]
    public partial class Player {
        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public int Avatar { get; set; }

        [DataMember]
        public PlayerStatus status;

        [IgnoreDataMember]
        public List<Player> Friends { get; set; }

        [IgnoreDataMember]
        public IPlayerManagerCallback PlayerManagerCallbackChannel { get; set; }

        public Player(DataAccess.Player playerData) {
            Nickname = playerData.Nickname;
            Avatar = playerData.Avatar;
        }
    }
}
