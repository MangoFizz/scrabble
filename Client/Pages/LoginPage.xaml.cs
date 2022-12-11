using Client.GameService;
using Client.Pages;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class LoginPage : Page {
        private CodeInputPage CodeInputPage { get; set; }

        private void HideTextMessages() {
            usernameRequiredText.Visibility = Visibility.Hidden;
            passwordRequiredText.Visibility = Visibility.Hidden;
            ResultText.Visibility = Visibility.Hidden;
        }

        private void ShowCodeInputPopup() {
            string label = Properties.Resources.SIGN_UP_MENU_VERIFICATION_CODE_LABEL;
            string message = Properties.Resources.SIGN_UP_MENU_VERIFICATION_CODE_MESSAGE;
            CodeInputPage = new CodeInputPage(label, message, (code) => {
                var username = usernameTextBox.Text;
                var password = passwordPasswordBox.Password;
                App.Current.PlayerManagerClient.VerifyPlayer(username, password, code);
            });
            App.Current.SecondaryFrame.Content = CodeInputPage;
        }

        public void ShowDisconnectMessage(DisconnectionReason reason) {
            ResultText.Visibility = Visibility.Visible;
            switch(reason) {
                case DisconnectionReason.ServerShutdown:
                    ResultText.Content = Properties.Resources.LOGIN_MENU_SERVER_SHUTDOWN_MESSAGE;
                    break;
                case DisconnectionReason.DuplicatePlayerSession:
                    ResultText.Content = Properties.Resources.LOGIN_MENU_DUPLICATE_SESSION_MESSAGE;
                    break;
            }
        }

        public void ShowLoginMessage(PlayerManagerPlayerAuthResult loginResult) {
            ResultText.Visibility = Visibility.Visible;
            switch(loginResult) {
                case PlayerManagerPlayerAuthResult.Success:
                    break;

                case PlayerManagerPlayerAuthResult.InvalidCredentials:
                    ResultText.Content = Properties.Resources.LOGIN_INVALID_CREDENTIALS_MESSAGE;
                    break;

                case PlayerManagerPlayerAuthResult.IncorrectPassword:
                    ResultText.Content = Properties.Resources.LOGIN_INCORRECT_PASSWORD_MESSAGE;
                    break;

                case PlayerManagerPlayerAuthResult.NotVerified:
                    ResultText.Content = Properties.Resources.LOGIN_MENU_USER_NOT_VERIFIED;
                    var username = usernameTextBox.Text;
                    var password = passwordPasswordBox.Password;
                    App.Current.PlayerManagerClient.ResendVerificationCode(username, password);
                    ShowCodeInputPopup();
                    break;

                default:
                    ResultText.Content = Properties.Resources.COMMON_UNKNOWN_ERROR;
                    break;
            }
        }

        private bool ValidateInput() {
            HideTextMessages();
            bool valid = true;
            if(usernameTextBox.Text.Length == 0) {
                usernameRequiredText.Visibility = Visibility.Visible;
                valid = false;
            }
            if(passwordPasswordBox.Password.Length == 0) {
                passwordRequiredText.Visibility = Visibility.Visible;
                valid = false;
            }
            return valid;
        }

        private void ShowPleaseWaitText() {
            ResultText.Content = Properties.Resources.COMMON_PLEASE_WAIT;
            ResultText.Visibility = Visibility.Visible;
        }

        private void Login() {
            if(ValidateInput()) {
                var username = usernameTextBox.Text;
                var password = passwordPasswordBox.Password;
                App.Current.PlayerManagerClient.Login(username, password);
                ShowPleaseWaitText();
            }
        }

        public LoginPage() {
            InitializeComponent();
            LanguageButton.Content = App.Current.CurrentLanguage;
            HideTextMessages();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            Login();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            var signupScreen = new SignUpPage();
            NavigationService.Navigate(signupScreen);
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e) {
            App.Current.SwitchLanguage();
            LanguageButton.Content = App.Current.CurrentLanguage;
            App.Current.MainWindow.MainFrame.Content = new LoginPage();
        }

        private void LoginAsGuestButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PlayerManagerClient.LoginAsGuest();
        }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult) {
            ShowLoginMessage(loginResult);
        }

        public void VerificationResponseHandler(PlayerManagerPlayerVerificationResult verificationResult) {
            switch(verificationResult) {
                case PlayerManagerPlayerVerificationResult.Success:
                    App.Current.CloseSecondaryPanel();
                    CodeInputPage = null;
                    Login();
                    break;

                case PlayerManagerPlayerVerificationResult.InvalidCode:
                    CodeInputPage.SetErrorMessage(Properties.Resources.CODE_INPUT_MENU_INVALID_CODE);
                    break;

                default:
                    CodeInputPage.SetErrorMessage(Properties.Resources.COMMON_UNKNOWN_ERROR);
                    break;
            }
        }
    }
}
