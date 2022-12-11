using Client.GameService;
using Client.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

        public Frame SecondaryFrame {
            get {
                return MainWindow.SecondaryFrame;
            }
        }

        public void ResetApp() {
            _PlayerManagerClient = null;
            Player = null;
            SessionId = null;
            CurrentParty = null;
            _PartyManagerClient = null;
            var loginPage = new LoginPage();
            MainWindow.MainFrame.Content = loginPage;
        }

        private static void CurrentDomain_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.Exception;
            Trace.TraceError($"ERROR: {e.GetType().Name} -> {e.Message}");
            Current.SecondaryFrame.Content = new DeadScreenPage();
            args.Handled = true;
        }

        App() {
            CurrentLanguage = languages[0];
            DispatcherUnhandledException += CurrentDomain_UnhandledException;
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

        public BitmapImage GetPlayerAvatarImage(Player player = null) {
            if(player == null) {
                player = Player;
            }
            var image = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{player.Avatar}.png"));
            return image;
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
                PartyManagerClient.ConnectPartyManager(SessionId);
                MainFrame.Content = new MainPage();
            }
            else {
                if(typeof(LoginPage).IsInstanceOfType(MainFrame.Content)) {
                    var loginPage = ((LoginPage)MainFrame.Content);
                    loginPage.LoginResponseHandler(loginResult);
                }
            }
        }

        public void ReceiveFriendAdd(Player player) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.ReceiveFriendAdd(player);
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
                    friendListPage.ReceiveFriendRequest(player);
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

        public void OnMainWindowClose() {
            if(Player != null) {
                PlayerManagerClient.Logout();
            }
        }

        public void UpdateFriendStatus(Player friend, Player.StatusType status) {
            if(MainWindow.FriendListFrame.Content != null) {
                var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
                if(friendListPage != null) {
                    friendListPage.UpdateFriendStatus(friend, status);
                }
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = ((PartyLobbyPage)MainFrame.Content);
                partyLobbyPage.UpdateFriendStatus(friend, status);
            }
        }

        public void Disconnect(DisconnectionReason reason) {
            ResetApp();
            var loginPage = (LoginPage)MainFrame.Content;
            loginPage.ShowDisconnectMessage(reason);
        }

        public void VerificationResponseHandler(PlayerManagerPlayerVerificationResult verificationResult) {
            if(typeof(LoginPage).IsInstanceOfType(MainFrame.Content)) {
                var loginPage = ((LoginPage)MainFrame.Content);
                loginPage.VerificationResponseHandler(verificationResult);
            }
            if(typeof(SignUpPage).IsInstanceOfType(MainFrame.Content)) {
                var signupPage = ((SignUpPage)MainFrame.Content);
                signupPage.VerificationResponseHandler(verificationResult);
            }
        }

        public void UpdatePlayerAvatarCallback(short avatarId) {
            Player.Avatar = avatarId;
            if(typeof(MainPage).IsInstanceOfType(MainFrame.Content)) {
                var mainPage = ((MainPage)MainFrame.Content);
                mainPage.LoadProfileButton();
            }
            if(typeof(PlayerProfilePage).IsInstanceOfType(SecondaryFrame.Content)) {
                var profilePage = ((PlayerProfilePage)SecondaryFrame.Content);
                profilePage.LoadProfileData();
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

        public void CreateParty() {
            CurrentParty = PartyManagerClient.CreateParty();
        }

        public void AcceptInvitationCallback(Party party) {
            CurrentParty = party;
            MainFrame.NavigationService.Navigate(new PartyLobbyPage());
        }

        public void JoinPartyCallback(JoinPartyResult result, Party party) {
            if(result == JoinPartyResult.Success) {
                CurrentParty = party;
                CloseSecondaryPanel();
                MainFrame.NavigationService.Navigate(new PartyLobbyPage());
            }
            else if(typeof(MainPage).IsInstanceOfType(MainFrame.Content)) {
                var mainPage = (MainPage)MainFrame.Content;
                mainPage.JoinPartyCallback(result, party);
            }
        }

        public void ReceiveGameStart() {
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceiveGameStart();
            }
        }

        public void ReceiveInvitation(Player player, string partyId) {
            var notificationsPage = (NotificationSidePage)MainWindow.NotificationsFrame.Content;
            notificationsPage.PushInviteNotification(new PartyInvitationPage(player));
        }

        public void ReceiveInvitationDecline(Player player) {
            throw new System.NotImplementedException();
        }

        public void ReceivePartyKick(Player player) {
            MainFrame.Content = new MainPage();
            CurrentParty = null;
        }

        public void ReceivePartyLanguageUpdate(GameSupportedLanguage language) {
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyLanguageUpdate(language);
            }
        }

        public void ReceivePartyLeaderTransfer(Player player) {
            var localPlayer = CurrentParty.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
            if(localPlayer != null) {
                CurrentParty.Leader = localPlayer;
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyLeaderTransfer(player);
            }
        }

        public void ReceivePartyPlayerJoin(Player player) {
            if(CurrentParty.Players != null) {
                CurrentParty.Players = CurrentParty.Players.Concat(new[] { player }).ToArray();
            }
            else {
                CurrentParty.Players = new[] { player };
            }

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyPlayerJoin(player);
            }
        }

        public void ReceivePartyPlayerLeave(Player player) {
            var localPlayer = CurrentParty.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
            CurrentParty.Players = CurrentParty.Players.Except(new[] { localPlayer }).ToArray();

            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyPlayerLeave(player);
            }
        }

        public void ReceivePartyTimeLimitUpdate(int time) {
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.ReceivePartyTimeLimitUpdate(time);
            }
        }

        public void StartGameCallback(GameStartResult result) {
            if(typeof(PartyLobbyPage).IsInstanceOfType(MainFrame.Content)) {
                var partyLobbyPage = (PartyLobbyPage)MainFrame.Content;
                partyLobbyPage.StartGameCallback(result);
            }
        }
    }
}
