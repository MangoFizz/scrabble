using Client.GameService;
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

    public partial class App : GameService.IPlayerManagerCallback {
        public Player LoggedPlayer { get; set; }

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
            throw new System.NotImplementedException();
        }
    }
}
