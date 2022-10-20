﻿using Client.Service;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    /// <summary>
    /// Lógica de interacción para login.xaml
    /// </summary>

    public partial class login : Page, Service.IAuthManagerCallback {
        private void hideTextMessages() {
            this.usernameRequiredText.Visibility = Visibility.Hidden;
            this.passwordRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public login() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
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
                    SignUp signupScreen = new SignUp();
                    this.NavigationService.Navigate(signupScreen);
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
