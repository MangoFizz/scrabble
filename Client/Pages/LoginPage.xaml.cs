using Client.GameService;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class LoginPage : Page {
        private void HideTextMessages() {
            this.usernameRequiredText.Visibility = Visibility.Hidden;
            this.passwordRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public LoginPage() {
            InitializeComponent();
            LanguageButton.Content = App.Current.CurrentLanguage;
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

                App.Current.PlayerManagerClient.Login(username, password);
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
                    this.resultText.Content = Properties.Resources.LOGIN_INVALID_CREDENTIALS_MESSAGE;
                    break;

                case PlayerManagerPlayerAuthResult.IncorrectPassword:
                    this.resultText.Content = Properties.Resources.LOGIN_INCORRECT_PASSWORD_MESSAGE;
                    break;

                default:
                    this.resultText.Content = Properties.Resources.COMMON_UNKNOWN_ERROR;
                    break;
            }
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e) {
            App.Current.SwitchLanguage();
            LanguageButton.Content = App.Current.CurrentLanguage;
            App.Current.MainWindow.MainFrame.Content = new LoginPage();
        }

        private void LoginAsGuessButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PlayerManagerClient.LoginAsGuest();
        }
    }
}
