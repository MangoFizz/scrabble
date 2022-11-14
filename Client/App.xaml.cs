using Client.GameService;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;

namespace Client {
    public partial class App : Application {
        private List<string> languages = new List<string>() { "en-US", "es-MX" };
        private string _CurrentLanguage;
        public string CurrentLanguage {
            get {
                return _CurrentLanguage;
            }
            private set {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(value);
                _CurrentLanguage = value;
            }
        }

        public static new App Current { 
            get {
                return (App)Application.Current;
            }
        }

        public new MainWindow MainWindow {
            get {
                return (MainWindow)base.MainWindow;
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

        public void SwitchLanguage() {
            var currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var nextLanguage = languages[(languages.IndexOf(currentLanguage) + 1) % languages.Count];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(nextLanguage);
            CurrentLanguage = nextLanguage;
        }
    }

    public partial class App : IPlayerManagerCallback {
        private PlayerManagerClient _PlayerManagerClient = null;

        public Player LoggedPlayer { get; set; }
        
        public PlayerManagerClient PlayerManagerClient { 
            get {
                if(_PlayerManagerClient == null) {
                    var context = new InstanceContext(App.Current);
                    var service = new PlayerManagerClient(context);
                    _PlayerManagerClient = service;
                }
                return _PlayerManagerClient;
            }
        }

        public void GetFriendListResponseHandler(Player[] friends) {
            var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
            friendListPage.FriendList = friends.ToList();
            friendListPage.RefreshFriendList();
        }

        public void GetFriendRequestsResponseHandler(Player[] friendRequests) {
            var friendListPage = ((FriendsListPage)MainWindow.FriendListFrame.Content);
            friendListPage.FriendRequests = friendRequests.ToList();
            friendListPage.RefreshFriendList();
        }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult, Player player) {
            if(loginResult == PlayerManagerPlayerAuthResult.Success) {
                LoggedPlayer = player;
                MainWindow.MainFrame.Content = new MainPage();
            }
            else {
                var loginPage = ((LoginPage)MainWindow.MainFrame.Content);
                loginPage.LoginResponse(loginResult);
            }
        }

        public void ReceiveFriendAdd(Player player) {
            throw new System.NotImplementedException();
        }

        public void ReceiveFriendRequest(Player player) {
            throw new System.NotImplementedException();
        }

        public void RegisterPlayerResponseHandler(PlayerManagerPlayerResgisterResult registrationResult) {
            var signUpPage = ((SignUpPage)MainWindow.MainFrame.Content);
            signUpPage.RegisterPlayerResponse(registrationResult);
        }

        public void SendFriendRequestResponseHandler(PlayerManagerPlayerFriendRequestResult result) {
            
        }
    }
}
