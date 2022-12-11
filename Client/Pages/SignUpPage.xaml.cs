using Client.GameService;
using Client.Pages;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class SignUpPage : Page {
        private CodeInputPage CodeInputPage { get; set; }
        
        private void HideTextMessages() {
            nicknameInvalidMessage.Visibility = Visibility.Hidden;
            emailInvalidMessage.Visibility = Visibility.Hidden;
            passwordInvalidMessage.Visibility = Visibility.Hidden;
            confirmPasswordInvalidMessage.Visibility = Visibility.Hidden;
            ResultText.Visibility = Visibility.Hidden;
        }

        private bool ValidateInputs() {
            bool isInputValid = true;
            var validCharactersRegex = new Regex("^[a-zA-Z0-9 ]*$");
            var passwordValidCharactersRegex = new Regex("^[a-zA-Z0-9!\" \"# $% & '() * +, -. / :; <=>? @ \\ [\\] \\ ^ _` {|} ~]*$");
            var containNumbersRegex = new Regex(@"(?=.*\d)");
            var containLowercaseRegex = new Regex(@"(?=.*[a-z])");
            var containUppercaseRegex = new Regex(@"(?=.*[A-Z])");

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
            else if(nicknameTextBox.Text.Length < 4) {
                nicknameInvalidMessage.Visibility = Visibility.Visible;
                nicknameInvalidMessage.Content = Properties.Resources.SIGNUP_NICKNAME_TOO_SHORT;
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
            else if(passwordTextBox.Password.Length < 8) {
                passwordInvalidMessage.Visibility = Visibility.Visible;
                passwordInvalidMessage.Content = Properties.Resources.SIGNUP_PASSWORD_TOO_SHORT;
                isInputValid = false;
            }
            else if(passwordValidCharactersRegex.IsMatch(passwordTextBox.Password) == false) {
                passwordInvalidMessage.Visibility = Visibility.Visible;
                passwordInvalidMessage.Content = Properties.Resources.COMMON_INVALID_CHARACTERS_LABEL;
                isInputValid = false;
            }
            else if(containLowercaseRegex.IsMatch(passwordTextBox.Password) == false) {
                ResultText.Visibility = Visibility.Visible;
                ResultText.Content = Properties.Resources.SIGNUP_PASSWORD_NO_LOWERCASE;
                isInputValid = false;
            }
            else if(containUppercaseRegex.IsMatch(passwordTextBox.Password) == false) {
                ResultText.Visibility = Visibility.Visible;
                ResultText.Content = Properties.Resources.SIGNUP_PASSWORD_NO_UPPERCASE;
                isInputValid = false;
            }
            else if(containNumbersRegex.IsMatch(passwordTextBox.Password) == false) {
                ResultText.Visibility = Visibility.Visible;
                ResultText.Content = Properties.Resources.SIGNUP_PASSWORD_NO_NUMBERS;
                isInputValid = false;
            }
            else {
                if(confirmPasswordTextBox.Password.Length > 0 && passwordTextBox.Password != confirmPasswordTextBox.Password) {
                    ResultText.Visibility = Visibility.Visible;
                    ResultText.Content = Properties.Resources.SIGNUP_PASSWORDS_DO_NOT_MATCH_LABEL;
                    isInputValid = false;
                }
            }

            if(confirmPasswordTextBox.Password.Length == 0) {
                confirmPasswordInvalidMessage.Visibility = Visibility.Visible;
                confirmPasswordInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }

            return isInputValid;
        }

        private void ShowVerificationCodeInputPopup() {
            string label = Properties.Resources.SIGN_UP_MENU_VERIFICATION_CODE_LABEL;
            string message = Properties.Resources.SIGN_UP_MENU_VERIFICATION_CODE_MESSAGE;
            CodeInputPage = new CodeInputPage(label, message, (code) => {
                var username = nicknameTextBox.Text;
                var password = passwordTextBox.Password;
                App.Current.PlayerManagerClient.VerifyPlayer(username, password, code);
            });
            App.Current.SecondaryFrame.Content = CodeInputPage;
        }

        private void ShowPleaseWaitText() {
            ResultText.Content = Properties.Resources.COMMON_PLEASE_WAIT;
            ResultText.Visibility = Visibility.Visible;
        }

        public SignUpPage() {
            InitializeComponent();
            HideTextMessages();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            HideTextMessages();
            if(ValidateInputs()) {
                var email = emailTextBox.Text;
                var nickname = nicknameTextBox.Text;
                var password = passwordTextBox.Password;
                App.Current.PlayerManagerClient.RegisterPlayer(nickname, password, email);
                ShowPleaseWaitText();
            }
        }

        public void RegisterPlayerResponse(GameService.PlayerManagerPlayerResgisterResult registrationResult) {
            ResultText.Visibility = Visibility.Visible;
            switch(registrationResult) {
                case PlayerManagerPlayerResgisterResult.Success:
                    ShowVerificationCodeInputPopup();
                    break;

                case PlayerManagerPlayerResgisterResult.PlayerAlreadyExists:
                    ResultText.Content = Properties.Resources.SIGN_UP_USER_ALREADY_EXISTS_MESSAGE;
                    break;

                default:
                    ResultText.Content = Properties.Resources.COMMON_UNKNOWN_ERROR;
                    break;
            }
        }

        public void VerificationResponseHandler(PlayerManagerPlayerVerificationResult verificationResult) {
            switch(verificationResult) {
                case PlayerManagerPlayerVerificationResult.Success:
                    App.Current.CloseSecondaryPanel();
                    NavigationService.GoBack();
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
