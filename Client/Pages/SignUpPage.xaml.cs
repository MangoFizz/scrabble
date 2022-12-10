using Client.GameService;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class SignUpPage : Page {
        private void HideTextMessages() {
            nicknameInvalidMessage.Visibility = Visibility.Hidden;
            emailInvalidMessage.Visibility = Visibility.Hidden;
            passwordInvalidMessage.Visibility = Visibility.Hidden;
            confirmPasswordInvalidMessage.Visibility = Visibility.Hidden;
            resultMessage.Visibility = Visibility.Hidden;
        }

        public SignUpPage() {
            InitializeComponent();
            HideTextMessages();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            bool isInputValid = true;
            
            HideTextMessages();

            var validCharactersRegex = new Regex("^[a-zA-Z0-9 ]*$");
            
            if(nicknameTextBox.Text.Length == 0) {
                nicknameInvalidMessage.Visibility = Visibility.Visible;
                nicknameInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }
            else if(nicknameTextBox.Text.Length > 12) {
                nicknameInvalidMessage.Visibility = Visibility.Visible;
                nicknameInvalidMessage.Content = Properties.Resources.SIGNUP_NICKNAME_TOO_LONG;
                isInputValid = false;
            }
            else if(validCharactersRegex.IsMatch(nicknameTextBox.Text) == false) {
                nicknameInvalidMessage.Visibility = Visibility.Visible;
                nicknameInvalidMessage.Content = Properties.Resources.COMMON_INVALID_CHARACTERS_LABEL;
                isInputValid = false;
            }

            if(emailTextBox.Text.Length == 0) {
                emailInvalidMessage.Visibility = Visibility.Visible;
                emailInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }
            else if(emailTextBox.Text.Length > 255) {
                emailInvalidMessage.Visibility = Visibility.Visible;
                emailInvalidMessage.Content = Properties.Resources.SIGNUP_EMAIL_TOO_LONG;
                isInputValid = false;
            }
            else {
                try {
                    new System.Net.Mail.MailAddress(emailTextBox.Text);
                }
                catch(FormatException) {
                    emailInvalidMessage.Visibility = Visibility.Visible;
                    emailInvalidMessage.Content = Properties.Resources.SIGN_UP_INVALID_EMAIL_LABEL;
                    isInputValid = false;
                }
            }

            if(passwordTextBox.Password.Length == 0) {
                passwordInvalidMessage.Visibility = Visibility.Visible;
                passwordInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }
            else if(passwordTextBox.Password.Length > 255) {
                passwordInvalidMessage.Visibility = Visibility.Visible;
                passwordInvalidMessage.Content = Properties.Resources.SIGNUP_PASSWORD_TOO_LONG;
                isInputValid = false;
            }
            else if(validCharactersRegex.IsMatch(passwordTextBox.Password) == false) {
                passwordInvalidMessage.Visibility = Visibility.Visible;
                passwordInvalidMessage.Content = Properties.Resources.COMMON_INVALID_CHARACTERS_LABEL;
                isInputValid = false;
            }

            if(confirmPasswordTextBox.Password.Length == 0) {
                confirmPasswordInvalidMessage.Visibility = Visibility.Visible;
                confirmPasswordInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }
            else if(passwordTextBox.Password != confirmPasswordTextBox.Password) {
                resultMessage.Visibility = Visibility.Visible;
                resultMessage.Content = Properties.Resources.SIGNUP_PASSWORDS_DO_NOT_MATCH_LABEL;
                isInputValid = false;
            }

            if(isInputValid) {
                var email = emailTextBox.Text;
                var nickname = nicknameTextBox.Text;
                var password = passwordTextBox.Password;

                App.Current.PlayerManagerClient.RegisterPlayer(nickname, password, email);
            }
        }

        public void RegisterPlayerResponse(GameService.PlayerManagerPlayerResgisterResult registrationResult) {
            resultMessage.Visibility = Visibility.Visible;
            switch(registrationResult) {
                case PlayerManagerPlayerResgisterResult.Success:
                    NavigationService.GoBack();
                    break;

                case PlayerManagerPlayerResgisterResult.PlayerAlreadyExists:
                    resultMessage.Content = Properties.Resources.SIGN_UP_USER_ALREADY_EXISTS_MESSAGE;
                    break;

                default:
                    resultMessage.Content = Properties.Resources.COMMON_UNKNOWN_ERROR;
                    break;
            }
        }
    }
}
