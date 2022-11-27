using Client.GameService;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class App : Application {
        private List<string> languages = new List<string>() { "en-US", "es-MX" };
        private string _CurrentLanguage;

        /// <summary>
        /// Current language reference
        /// </summary>
        public string CurrentLanguage {
            get {
                return _CurrentLanguage;
            }
            private set {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(value);
                _CurrentLanguage = value;
            }
        }

        /// <summary>
        /// Application instance
        /// </summary>
        public static new App Current { 
            get {
                return (App)Application.Current;
            }
        }

        /// <summary>
        /// Main window instance
        /// </summary>
        public new MainWindow MainWindow {
            get {
                return (MainWindow)base.MainWindow;
            }
        }

        public Frame MainFrame {
            get {
                return MainWindow.MainFrame;
            }
        }

        App() {
            CurrentLanguage = languages[0];
        }

        public void OpenFriendsList() {
            MainWindow.FriendListFrame.Content = new FriendsListPage();
        }

        public void CloseFriendsList() {
            MainWindow.FriendListFrame.Content = null;
        }

        public void OpenPlayerProfile() {
            MainWindow.SecondaryFrame.Content = new PlayerProfilePage(Player);
        }

        public void CloseSecondaryPanel() {
            MainWindow.SecondaryFrame.Content = null;
        }

        public void SwitchLanguage() {
            var currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var nextLanguage = languages[(languages.IndexOf(currentLanguage) + 1) % languages.Count];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(nextLanguage);
            CurrentLanguage = nextLanguage;
        }
    }

    public partial class App : IPlayerManagerCallback {
        private PlayerManagerClient _PlayerManagerClient = null;

        /// <summary>
        /// Current logged in player
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Current session ID
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Service client for player manager
        /// </summary>
        public PlayerManagerClient PlayerManagerClient { 
            get {
                if(_PlayerManagerClient == null) {
                    var context = new InstanceContext(this);
                    var service = new PlayerManagerClient(context);
                    _PlayerManagerClient = service;
                }
                return _PlayerManagerClient;
            }
        }

        public void GetFriendListResponseHandler(Player[] friends) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                friendListPage.Friends = friends.ToList();
                friendListPage.RefreshFriendList();
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = ((PartyLobbyPage)MainFrame.Content);
                partyLobbyPage.GetFriendListResponseHandler(friends);
            }
        }

            public void GetFriendRequestsResponseHandler(Player[] friendRequests) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                friendListPage.FriendRequests = friendRequests.ToList();
                friendListPage.RefreshFriendList();
            }
        }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult, Player player, string sessionId) {
            if(loginResult == PlayerManagerPlayerAuthResult.Success) {
                Player = player;
                SessionId = sessionId;
                PartyManagerClient.Subscribe(SessionId);
                MainFrame.Content = new MainPage();
            }
            else {
                if(typeof(LoginPage).IsInstanceOfType(MainFrame.Content)) {
                    var loginPage = ((LoginPage)MainFrame.Content);
                    loginPage.LoginResponse(loginResult);
                }
            }
        }

        public void ReceiveFriendAdd(Player player) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.FriendAddHandler(player);
                }
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = ((PartyLobbyPage)MainFrame.Content);
                partyLobbyPage.ReceiveFriendAdd(player);
            }
        }

        public void ReceiveFriendRequest(Player player) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.FriendRequestReceiveHandler(player);
                }
            }
        }

        public void RegisterPlayerResponseHandler(PlayerManagerPlayerResgisterResult registrationResult) {
            if(typeof(SignUpPage).IsInstanceOfType(MainFrame.Content)) {
                var signUpPage = ((SignUpPage)MainWindow.MainFrame.Content);
                signUpPage.RegisterPlayerResponse(registrationResult);
            }
        }

        public void SendFriendRequestResponseHandler(PlayerManagerPlayerFriendRequestResult result) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.SendFriendRequestResponseHandler(result);
                }
            }
        }

        public void FriendConnect(Player player) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.FriendConnect(player);
                }
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = ((PartyLobbyPage)MainFrame.Content);
                partyLobbyPage.FriendConnect(player);
            }
        }

        public void FriendDisconnect(Player player) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.FriendDisconnect(player);
                }
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = ((PartyLobbyPage)MainFrame.Content);
                partyLobbyPage.FriendDisconnect(player);
            }
        }

        public void OnMainWindowClose() {
            if(Player != null) {
                //PlayerManagerClient.Logout();
            }
        }
    }

    public partial class App : IPartyManagerCallback {
        private PartyManagerClient _PartyManagerClient = null;

        public Party CurrentParty { get; set; }

        public PartyManagerClient PartyManagerClient {
            get {
                if(_PartyManagerClient == null) {
                    var context = new InstanceContext(this);
                    var service = new PartyManagerClient(context);
                    _PartyManagerClient = service;
                }
                return _PartyManagerClient;
            }
        }

        public void AcceptInvitationCallback(Party party) {
            throw new System.NotImplementedException();
        }

        public void CreatePartyCallback(Party party) {
            CurrentParty = party;
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.CreatePartyCallback(party);
            }
        }

        public void ReceiveGameCancel() {
            throw new System.NotImplementedException();
        }

        public void ReceiveGameStart() {
            throw new System.NotImplementedException();
        }

        public void ReceiveInvitation(Player player, string partyId) {
            throw new System.NotImplementedException();
        }

        public void ReceiveInvitationDecline(Player player) {
            throw new System.NotImplementedException();
        }

        public void ReceivePartyKick(Player player) {
            throw new System.NotImplementedException();
        }

        public void ReceivePartyLeaderTransfer(Player player) {
            throw new System.NotImplementedException();
        }

        public void ReceivePartyPlayerJoin(Player player) {
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyPlayerJoin(player);
            }
        }

        public void ReceivePartyPlayerLeave(Player player) {
            throw new System.NotImplementedException();
        }
    }
}
