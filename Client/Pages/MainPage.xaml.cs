using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client {
    public partial class MainPage : Page {
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

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }
    }
}
