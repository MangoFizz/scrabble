using Client.GameService;
using System.Linq;
using System.ServiceModel;
using System.Windows;

namespace Client {
    public partial class App : Application {
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
            // Set locale
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
        }

        public void OpenFriendsList() {
            MainWindow.FriendListFrame.Content = new FriendsListPage();
        }

        public void CloseFriendsList() {
            MainWindow.FriendListFrame.Content = null;
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
                MainWindow.MainFrame.Content = new Main();
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
