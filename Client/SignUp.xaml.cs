using Client.AuthManagerService;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    /// <summary>
    /// Lógica de interacción para SignUp.xaml
    /// </summary>
    public partial class SignUp : Page, AuthManagerService.IAuthManagerCallback {
        private void hideTextMessages() {
            this.nameFieldRequiredText.Visibility = Visibility.Hidden;
            this.surnameFieldRequiredText.Visibility = Visibility.Hidden;
            this.emailFieldRequiredText.Visibility = Visibility.Hidden;
            this.nicknameFieldRequiredText.Visibility = Visibility.Hidden;
            this.passwordFieldRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public SignUp() {
            InitializeComponent();
            this.hideTextMessages();
        }

        private void cancelButtonClick(object sender, RoutedEventArgs e) {
            this.NavigationService.GoBack();
        }

        private void acceptButtonClick(object sender, RoutedEventArgs e) {
            bool isInputValid = true;

            this.hideTextMessages();

            if(this.nameTextBox.Text.Length == 0) {
                this.nameFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.surnameTextBox.Text.Length == 0) {
                this.surnameFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.emailTextBox.Text.Length == 0) {
                this.emailFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.nicknameTextBox.Text.Length == 0) {
                this.nicknameFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.passwordPasswordBox.Password.Length == 0) {
                this.passwordFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(isInputValid) {
                var name = this.nameTextBox.Text;
                var surname = this.surnameTextBox.Text;
                var email = this.emailTextBox.Text;
                var nickname = this.nicknameTextBox.Text;
                var password = this.passwordPasswordBox.Password;

                var context = new InstanceContext(this);
                var service = new AuthManagerService.AuthManagerClient(context);
                service.registerUser(name, password);
            }
        }

        public void loginResponse(AuthenticatorUserAuthResult loginResult) {
            throw new System.NotImplementedException();
        }

        public void registerUserResponse(AuthenticatorUserResgisterResult registrationResult) {
            this.resultText.Visibility = Visibility.Visible;
            switch(registrationResult) {
                case AuthenticatorUserResgisterResult.Success:
                    this.NavigationService.GoBack();
                    break;

                case AuthenticatorUserResgisterResult.UserAlreadyExists:
                    this.resultText.Content = Properties.Resources.userAlreadyExists;
                    break;

                default:
                    this.resultText.Content = Properties.Resources.unknownError;
                    break;
            }
        }
    }
}
