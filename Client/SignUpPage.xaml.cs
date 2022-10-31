using Client.GameService;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class SignUpPage : Page {
        private void HideTextMessages() {
            this.emailFieldRequiredText.Visibility = Visibility.Hidden;
            this.nicknameFieldRequiredText.Visibility = Visibility.Hidden;
            this.passwordFieldRequiredText.Visibility = Visibility.Hidden;
            this.resultText.Visibility = Visibility.Hidden;
        }

        public SignUpPage() {
            InitializeComponent();
            this.HideTextMessages();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            this.NavigationService.GoBack();
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e) {
            bool isInputValid = true;

            this.HideTextMessages();
            
            if(this.nicknameTextBox.Text.Length == 0) {
                this.nicknameFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.passwordPasswordBox.Password.Length == 0) {
                this.passwordFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(this.emailTextBox.Text.Length == 0) {
                this.emailFieldRequiredText.Visibility = Visibility.Visible;
                isInputValid = false;
            }

            if(isInputValid) {
                var email = this.emailTextBox.Text;
                var nickname = this.nicknameTextBox.Text;
                var password = this.passwordPasswordBox.Password;

                App.Current.PlayerManagerClient.RegisterPlayer(nickname, password, email);
            }
        }

        public void RegisterPlayerResponse(GameService.PlayerManagerPlayerResgisterResult registrationResult) {
            this.resultText.Visibility = Visibility.Visible;
            switch(registrationResult) {
                case PlayerManagerPlayerResgisterResult.Success:
                    this.NavigationService.GoBack();
                    break;

                case PlayerManagerPlayerResgisterResult.PlayerAlreadyExists:
                    this.resultText.Content = Properties.Resources.userAlreadyExists;
                    break;

                default:
                    this.resultText.Content = Properties.Resources.unknownError;
                    break;
            }
        }
    }
}
