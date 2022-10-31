using Client.GameService;
using System.Windows;

namespace Client {
    public partial class App : Application {
        App() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
        }
    }

    public partial class App : GameService.IPlayerManagerCallback {
        public Player LoggedPlayer { get; set; }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult, Player player) {
            var mainWindow = ((MainWindow)this.MainWindow);
            
            if(loginResult == PlayerManagerPlayerAuthResult.Success) {
                LoggedPlayer = player;
                mainWindow.MainFrame.Content = new Main();
            }
            else {
                var loginPage = ((LoginPage)mainWindow.MainFrame.Content);
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
            var mainWindow = ((MainWindow)this.MainWindow);
            var signUpPage = ((SignUpPage)mainWindow.MainFrame.Content);
            signUpPage.RegisterPlayerResponse(registrationResult);
        }

        public void SendFriendRequestResponseHandler(PlayerManagerPlayerFriendRequestResult result) {
            throw new System.NotImplementedException();
        }
    }
}
