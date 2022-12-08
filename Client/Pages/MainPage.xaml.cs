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
            PlayerProfileAvatar.Source = App.Current.GetPlayerAvatarImage();
            PlayerProfileName.Text = App.Current.Player.Nickname;
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
            App.Current.Shutdown();
        }
    }
}
