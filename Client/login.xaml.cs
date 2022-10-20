using Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Lógica de interacción para login.xaml
    /// </summary>

    public partial class login : Window, Service.IAuthManagerCallback {
        private void hideTextMessages() {
            this.usernameRequiredText.Visibility = Visibility.Hidden;
            this.passwordRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public login() {
            InitializeComponent();
            this.hideTextMessages();
        }

        private void loginButtonClick(object sender, RoutedEventArgs e) {
            bool isInputValid = true;

            this.hideTextMessages();

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

                var context = new InstanceContext(this);
                var service = new Service.AuthManagerClient(context);
                service.login(username, password);
            }
        }

        public void loginResponse(AuthenticatorUserAuthResult loginResult) {
            this.resultText.Visibility = Visibility.Visible;
            switch(loginResult) {
                case AuthenticatorUserAuthResult.Success:
                    this.resultText.Content = "Success!";        
                    break;

                case AuthenticatorUserAuthResult.InvalidCredentials:
                    this.resultText.Content = "Invalid credentials";
                    break;

                case AuthenticatorUserAuthResult.IncorrectPassword:
                    this.resultText.Content = "Incorrect password";
                    break;

                default:
                    this.resultText.Content = "Unknown error";
                    break;
            }

        }

        public void registerUserResponse(AuthenticatorUserResgisterResult registrationResult) {
            throw new NotImplementedException();
        }
    }
}
