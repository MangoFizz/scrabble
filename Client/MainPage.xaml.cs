using System.Windows;
using System.Windows.Controls;

namespace Client {
    public partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e) {
            App.Current.OpenFriendsList();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            App.Current.MainWindow.MainFrame.Navigate(new GameBoardPage());
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
