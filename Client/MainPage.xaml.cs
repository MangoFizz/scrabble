using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client {
    public partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();
            LoadProfileButton();
        }

        public void LoadProfileButton() {
            PlayerProfileName.Text = App.Current.Player.Nickname;
            PlayerProfileAvatar.Source = new BitmapImage(new Uri($"/Assets/images/avatars/default_{App.Current.Player.Avatar}.png", UriKind.Relative));
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e) {
            App.Current.OpenFriendsList();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e) {
            App.Current.OpenPlayerProfile();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            App.Current.MainWindow.MainFrame.Navigate(new PlayPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            App.Current.MainWindow.MainFrame.Navigate(new SettingsPage());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PlayerManagerClient.Logout();
            App.Current.Shutdown();
        }
    }
}
