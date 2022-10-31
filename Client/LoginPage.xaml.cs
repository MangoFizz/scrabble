using Client.GameService;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    /// <summary>
    /// Lógica de interacción para login.xaml
    /// </summary>

    public partial class LoginPage : Page {
        private void HideTextMessages() {
            this.usernameRequiredText.Visibility = Visibility.Hidden;
            this.passwordRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public LoginPage() {
            InitializeComponent();
            this.HideTextMessages();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e) {
            bool isInputValid = true;

            this.HideTextMessages();

            // Show text message if user leaves the username field empty
            if(this.usernameTextBox.Text.Length == 0) {
                this.usernameRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            // Show text message if user leaves the password field empty
            if(this.passwordPasswordBox.Password.Length == 0) {
                this.passwordRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(isInputValid) {
                var username = this.usernameTextBox.Text;
                var password = this.passwordPasswordBox.Password;

                var context = new InstanceContext(App.Current);
                var service = new GameService.PlayerManagerClient(context);
                service.Login(username, password);
            }
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e) {
            var signupScreen = new SignUpPage();
            this.NavigationService.Navigate(signupScreen);
        }

        public void LoginResponse(PlayerManagerPlayerAuthResult loginResult) {
            this.resultText.Visibility = Visibility.Visible;
            switch(loginResult) {
                case PlayerManagerPlayerAuthResult.Success:
                    break;

                case PlayerManagerPlayerAuthResult.InvalidCredentials:
                    this.resultText.Content = Properties.Resources.invalidCredentials;
                    break;

                case PlayerManagerPlayerAuthResult.IncorrectPassword:
                    this.resultText.Content = Properties.Resources.incorrectPassword;
                    break;

                default:
                    this.resultText.Content = Properties.Resources.unknownError;
                    break;
            }
        }
    }
}
