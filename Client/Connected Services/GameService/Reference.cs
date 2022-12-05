﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client.GameService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerManager.PlayerAuthResult", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum PlayerManagerPlayerAuthResult : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidCredentials = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IncorrectPassword = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        DatabaseError = 3,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Player", Namespace="http://schemas.datacontract.org/2004/07/Service")]
    [System.SerializableAttribute()]
    public partial class Player : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AvatarField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GamesCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NicknameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime RegisteredField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int WinsCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Client.GameService.PlayerStatus statusField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Avatar {
            get {
                return this.AvatarField;
            }
            set {
                if ((this.AvatarField.Equals(value) != true)) {
                    this.AvatarField = value;
                    this.RaisePropertyChanged("Avatar");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email {
            get {
                return this.EmailField;
            }
            set {
                if ((object.ReferenceEquals(this.EmailField, value) != true)) {
                    this.EmailField = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int GamesCount {
            get {
                return this.GamesCountField;
            }
            set {
                if ((this.GamesCountField.Equals(value) != true)) {
                    this.GamesCountField = value;
                    this.RaisePropertyChanged("GamesCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nickname {
            get {
                return this.NicknameField;
            }
            set {
                if ((object.ReferenceEquals(this.NicknameField, value) != true)) {
                    this.NicknameField = value;
                    this.RaisePropertyChanged("Nickname");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Registered {
            get {
                return this.RegisteredField;
            }
            set {
                if ((this.RegisteredField.Equals(value) != true)) {
                    this.RegisteredField = value;
                    this.RaisePropertyChanged("Registered");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int WinsCount {
            get {
                return this.WinsCountField;
            }
            set {
                if ((this.WinsCountField.Equals(value) != true)) {
                    this.WinsCountField = value;
                    this.RaisePropertyChanged("WinsCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Client.GameService.PlayerStatus status {
            get {
                return this.statusField;
            }
            set {
                if ((this.statusField.Equals(value) != true)) {
                    this.statusField = value;
                    this.RaisePropertyChanged("status");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerStatus", Namespace="http://schemas.datacontract.org/2004/07/Service")]
    public enum PlayerStatus : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Offline = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Online = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InGame = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerManager.PlayerResgisterResult", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum PlayerManagerPlayerResgisterResult : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PlayerAlreadyExists = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidInputs = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        DatabaseError = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerManager.PlayerFriendRequestResult", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum PlayerManagerPlayerFriendRequestResult : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SelfRequest = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AlreadyFriends = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PendingRequest = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SenderPlayerDoesNotExists = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ReceiverPlayerDoesNotExists = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        DatabaseError = 6,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Game.Language", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum GameLanguage : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        en_US = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        es_MX = 1,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Party", Namespace="http://schemas.datacontract.org/2004/07/Service")]
    [System.SerializableAttribute()]
    public partial class Party : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Client.GameService.Player LeaderField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Client.GameService.Player[] PlayersField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id {
            get {
                return this.IdField;
            }
            set {
                if ((object.ReferenceEquals(this.IdField, value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Client.GameService.Player Leader {
            get {
                return this.LeaderField;
            }
            set {
                if ((object.ReferenceEquals(this.LeaderField, value) != true)) {
                    this.LeaderField = value;
                    this.RaisePropertyChanged("Leader");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Client.GameService.Player[] Players {
            get {
                return this.PlayersField;
            }
            set {
                if ((object.ReferenceEquals(this.PlayersField, value) != true)) {
                    this.PlayersField = value;
                    this.RaisePropertyChanged("Players");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameService.IPlayerManager", CallbackContract=typeof(Client.GameService.IPlayerManagerCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IPlayerManager {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/Login")]
        void Login(string nickname, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/Login")]
        System.Threading.Tasks.Task LoginAsync(string nickname, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/RegisterPlayer")]
        void RegisterPlayer(string nickname, string password, string email);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/RegisterPlayer")]
        System.Threading.Tasks.Task RegisterPlayerAsync(string nickname, string password, string email);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, Action="http://tempuri.org/IPlayerManager/Logout")]
        void Logout();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, Action="http://tempuri.org/IPlayerManager/Logout")]
        System.Threading.Tasks.Task LogoutAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendList")]
        void GetFriendList();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendList")]
        System.Threading.Tasks.Task GetFriendListAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendRequests")]
        void GetFriendRequests();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendRequests")]
        System.Threading.Tasks.Task GetFriendRequestsAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/SendFriendRequest")]
        void SendFriendRequest(string nickname);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/SendFriendRequest")]
        System.Threading.Tasks.Task SendFriendRequestAsync(string nickname);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/AcceptFriendRequest")]
        void AcceptFriendRequest(string nickname);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/AcceptFriendRequest")]
        System.Threading.Tasks.Task AcceptFriendRequestAsync(string nickname);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/DeclineFriendRequest")]
        void DeclineFriendRequest(string nickname);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/DeclineFriendRequest")]
        System.Threading.Tasks.Task DeclineFriendRequestAsync(string nickname);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPlayerManagerCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/LoginResponseHandler")]
        void LoginResponseHandler(Client.GameService.PlayerManagerPlayerAuthResult loginResult, Client.GameService.Player player, string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/RegisterPlayerResponseHandler")]
        void RegisterPlayerResponseHandler(Client.GameService.PlayerManagerPlayerResgisterResult registrationResult);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/SendFriendRequestResponseHandler")]
        void SendFriendRequestResponseHandler(Client.GameService.PlayerManagerPlayerFriendRequestResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendListResponseHandler")]
        void GetFriendListResponseHandler(Client.GameService.Player[] friends);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/GetFriendRequestsResponseHandler")]
        void GetFriendRequestsResponseHandler(Client.GameService.Player[] friendRequests);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/ReceiveFriendRequest")]
        void ReceiveFriendRequest(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/ReceiveFriendAdd")]
        void ReceiveFriendAdd(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/FriendConnect")]
        void FriendConnect(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPlayerManager/FriendDisconnect")]
        void FriendDisconnect(Client.GameService.Player player);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPlayerManagerChannel : Client.GameService.IPlayerManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PlayerManagerClient : System.ServiceModel.DuplexClientBase<Client.GameService.IPlayerManager>, Client.GameService.IPlayerManager {
        
        public PlayerManagerClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public PlayerManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public PlayerManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PlayerManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PlayerManagerClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Login(string nickname, string password) {
            base.Channel.Login(nickname, password);
        }
        
        public System.Threading.Tasks.Task LoginAsync(string nickname, string password) {
            return base.Channel.LoginAsync(nickname, password);
        }
        
        public void RegisterPlayer(string nickname, string password, string email) {
            base.Channel.RegisterPlayer(nickname, password, email);
        }
        
        public System.Threading.Tasks.Task RegisterPlayerAsync(string nickname, string password, string email) {
            return base.Channel.RegisterPlayerAsync(nickname, password, email);
        }
        
        public void Logout() {
            base.Channel.Logout();
        }
        
        public System.Threading.Tasks.Task LogoutAsync() {
            return base.Channel.LogoutAsync();
        }
        
        public void GetFriendList() {
            base.Channel.GetFriendList();
        }
        
        public System.Threading.Tasks.Task GetFriendListAsync() {
            return base.Channel.GetFriendListAsync();
        }
        
        public void GetFriendRequests() {
            base.Channel.GetFriendRequests();
        }
        
        public System.Threading.Tasks.Task GetFriendRequestsAsync() {
            return base.Channel.GetFriendRequestsAsync();
        }
        
        public void SendFriendRequest(string nickname) {
            base.Channel.SendFriendRequest(nickname);
        }
        
        public System.Threading.Tasks.Task SendFriendRequestAsync(string nickname) {
            return base.Channel.SendFriendRequestAsync(nickname);
        }
        
        public void AcceptFriendRequest(string nickname) {
            base.Channel.AcceptFriendRequest(nickname);
        }
        
        public System.Threading.Tasks.Task AcceptFriendRequestAsync(string nickname) {
            return base.Channel.AcceptFriendRequestAsync(nickname);
        }
        
        public void DeclineFriendRequest(string nickname) {
            base.Channel.DeclineFriendRequest(nickname);
        }
        
        public System.Threading.Tasks.Task DeclineFriendRequestAsync(string nickname) {
            return base.Channel.DeclineFriendRequestAsync(nickname);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameService.IPartyChat", CallbackContract=typeof(Client.GameService.IPartyChatCallback))]
    public interface IPartyChat {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Connect")]
        void Connect(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Connect")]
        System.Threading.Tasks.Task ConnectAsync(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Say")]
        void Say(string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Say")]
        System.Threading.Tasks.Task SayAsync(string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Whisper")]
        void Whisper(Client.GameService.Player receiver, string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyChat/Whisper")]
        System.Threading.Tasks.Task WhisperAsync(Client.GameService.Player receiver, string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPartyChatCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyChat/Receive", ReplyAction="http://tempuri.org/IPartyChat/ReceiveResponse")]
        void Receive(Client.GameService.Player sender, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyChat/ReceiveWhisper", ReplyAction="http://tempuri.org/IPartyChat/ReceiveWhisperResponse")]
        void ReceiveWhisper(Client.GameService.Player sender, string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPartyChatChannel : Client.GameService.IPartyChat, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PartyChatClient : System.ServiceModel.DuplexClientBase<Client.GameService.IPartyChat>, Client.GameService.IPartyChat {
        
        public PartyChatClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public PartyChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public PartyChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PartyChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PartyChatClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Connect(string sessionId) {
            base.Channel.Connect(sessionId);
        }
        
        public System.Threading.Tasks.Task ConnectAsync(string sessionId) {
            return base.Channel.ConnectAsync(sessionId);
        }
        
        public void Say(string message) {
            base.Channel.Say(message);
        }
        
        public System.Threading.Tasks.Task SayAsync(string message) {
            return base.Channel.SayAsync(message);
        }
        
        public void Whisper(Client.GameService.Player receiver, string message) {
            base.Channel.Whisper(receiver, message);
        }
        
        public System.Threading.Tasks.Task WhisperAsync(Client.GameService.Player receiver, string message) {
            return base.Channel.WhisperAsync(receiver, message);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GameService.IPartyManager", CallbackContract=typeof(Client.GameService.IPartyManagerCallback))]
    public interface IPartyManager {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/Subscribe")]
        void Subscribe(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/Subscribe")]
        System.Threading.Tasks.Task SubscribeAsync(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/CreateParty")]
        void CreateParty();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/CreateParty")]
        System.Threading.Tasks.Task CreatePartyAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/LeaveParty")]
        void LeaveParty();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/LeaveParty")]
        System.Threading.Tasks.Task LeavePartyAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/StartGame")]
        void StartGame(Client.GameService.GameLanguage language, int timeLimitMins);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/StartGame")]
        System.Threading.Tasks.Task StartGameAsync(Client.GameService.GameLanguage language, int timeLimitMins);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/CancelGame")]
        void CancelGame();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/CancelGame")]
        System.Threading.Tasks.Task CancelGameAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/InvitePlayer")]
        void InvitePlayer(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/InvitePlayer")]
        System.Threading.Tasks.Task InvitePlayerAsync(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/AcceptInvitation")]
        void AcceptInvitation(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/AcceptInvitation")]
        System.Threading.Tasks.Task AcceptInvitationAsync(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/KickPlayer")]
        void KickPlayer(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/KickPlayer")]
        System.Threading.Tasks.Task KickPlayerAsync(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/TransferLeadership")]
        void TransferLeadership(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPartyManager/TransferLeadership")]
        System.Threading.Tasks.Task TransferLeadershipAsync(Client.GameService.Player player);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPartyManagerCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/CreatePartyCallback", ReplyAction="http://tempuri.org/IPartyManager/CreatePartyCallbackResponse")]
        void CreatePartyCallback(Client.GameService.Party party);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceiveInvitation", ReplyAction="http://tempuri.org/IPartyManager/ReceiveInvitationResponse")]
        void ReceiveInvitation(Client.GameService.Player player, string partyId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/AcceptInvitationCallback", ReplyAction="http://tempuri.org/IPartyManager/AcceptInvitationCallbackResponse")]
        void AcceptInvitationCallback(Client.GameService.Party party);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceiveInvitationDecline", ReplyAction="http://tempuri.org/IPartyManager/ReceiveInvitationDeclineResponse")]
        void ReceiveInvitationDecline(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceivePartyPlayerLeave", ReplyAction="http://tempuri.org/IPartyManager/ReceivePartyPlayerLeaveResponse")]
        void ReceivePartyPlayerLeave(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceivePartyPlayerJoin", ReplyAction="http://tempuri.org/IPartyManager/ReceivePartyPlayerJoinResponse")]
        void ReceivePartyPlayerJoin(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceiveGameStart", ReplyAction="http://tempuri.org/IPartyManager/ReceiveGameStartResponse")]
        void ReceiveGameStart();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceiveGameCancel", ReplyAction="http://tempuri.org/IPartyManager/ReceiveGameCancelResponse")]
        void ReceiveGameCancel();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceivePartyKick", ReplyAction="http://tempuri.org/IPartyManager/ReceivePartyKickResponse")]
        void ReceivePartyKick(Client.GameService.Player player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPartyManager/ReceivePartyLeaderTransfer", ReplyAction="http://tempuri.org/IPartyManager/ReceivePartyLeaderTransferResponse")]
        void ReceivePartyLeaderTransfer(Client.GameService.Player player);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPartyManagerChannel : Client.GameService.IPartyManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PartyManagerClient : System.ServiceModel.DuplexClientBase<Client.GameService.IPartyManager>, Client.GameService.IPartyManager {
        
        public PartyManagerClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public PartyManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public PartyManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PartyManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PartyManagerClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Subscribe(string sessionId) {
            base.Channel.Subscribe(sessionId);
        }
        
        public System.Threading.Tasks.Task SubscribeAsync(string sessionId) {
            return base.Channel.SubscribeAsync(sessionId);
        }
        
        public void CreateParty() {
            base.Channel.CreateParty();
        }
        
        public System.Threading.Tasks.Task CreatePartyAsync() {
            return base.Channel.CreatePartyAsync();
        }
        
        public void LeaveParty() {
            base.Channel.LeaveParty();
        }
        
        public System.Threading.Tasks.Task LeavePartyAsync() {
            return base.Channel.LeavePartyAsync();
        }
        
        public void StartGame(Client.GameService.GameLanguage language, int timeLimitMins) {
            base.Channel.StartGame(language, timeLimitMins);
        }
        
        public System.Threading.Tasks.Task StartGameAsync(Client.GameService.GameLanguage language, int timeLimitMins) {
            return base.Channel.StartGameAsync(language, timeLimitMins);
        }
        
        public void CancelGame() {
            base.Channel.CancelGame();
        }
        
        public System.Threading.Tasks.Task CancelGameAsync() {
            return base.Channel.CancelGameAsync();
        }
        
        public void InvitePlayer(Client.GameService.Player player) {
            base.Channel.InvitePlayer(player);
        }
        
        public System.Threading.Tasks.Task InvitePlayerAsync(Client.GameService.Player player) {
            return base.Channel.InvitePlayerAsync(player);
        }
        
        public void AcceptInvitation(Client.GameService.Player player) {
            base.Channel.AcceptInvitation(player);
        }
        
        public System.Threading.Tasks.Task AcceptInvitationAsync(Client.GameService.Player player) {
            return base.Channel.AcceptInvitationAsync(player);
        }
        
        public void KickPlayer(Client.GameService.Player player) {
            base.Channel.KickPlayer(player);
        }
        
        public System.Threading.Tasks.Task KickPlayerAsync(Client.GameService.Player player) {
            return base.Channel.KickPlayerAsync(player);
        }
        
        public void TransferLeadership(Client.GameService.Player player) {
            base.Channel.TransferLeadership(player);
        }
        
        public System.Threading.Tasks.Task TransferLeadershipAsync(Client.GameService.Player player) {
            return base.Channel.TransferLeadershipAsync(player);
        }
    }
}
