using Client.GameService;
using Client.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client {
    public partial class MainPage : Page {
        private CodeInputPage CodeInputPage { get; set; }
        
        private void ShowPartyInviteCodeInput() {
            string label = Properties.Resources.PLAY_MENU_INVITE_CODE_LABEL;
            string message = Properties.Resources.PLAY_MENU_INVITE_CODE_MESSAGE;
            CodeInputPage = new CodeInputPage(label, message, (code) => {
                App.Current.PartyManagerClient.JoinParty(code);
            });
            App.Current.SecondaryFrame.Content = CodeInputPage;
        }

        public MainPage() {
            InitializeComponent();
            LoadProfileButton();

            if(App.Current.Player.IsGuest) {
                MainGrid.Children.Remove(FriendsButton);
                MainGrid.Children.Remove(ProfileButton);
            }
        }

        public void LoadProfileButton() {
            PlayerProfileAvatar.Source = App.Current.GetPlayerAvatarImage();
            PlayerProfileName.Text = App.Current.Player.Nickname;
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e) {
            App.Current.OpenFriendsList();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e) {
            App.Current.OpenPlayerProfile();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e) {
            App.Current.MainWindow.MainFrame.Navigate(new PartyLobbyPage());
        }

        private void JoinGameButton_Click(object sender, RoutedEventArgs e) {
            ShowPartyInviteCodeInput();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }

        public void JoinPartyCallback(JoinPartyResult result, Party party) {
            switch(result) {
                case JoinPartyResult.PartyNotFound:
                    CodeInputPage.SetErrorMessage(Properties.Resources.PLAY_MENU_PARTY_NOT_FOUND);
                    break;

                case JoinPartyResult.PartyIsFull:
                    CodeInputPage.SetErrorMessage(Properties.Resources.PLAY_MENU_PARTY_IS_FULL);
                    break;

                default:
                    CodeInputPage.SetErrorMessage(Properties.Resources.COMMON_UNKNOWN_ERROR);
                    break;
            }
        }
    }
}
