using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Interaction logic for PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page {
        public PlayPage() {
            InitializeComponent();
            LoadProfileButton();

            if(App.Current.Player.IsGuest) {
                MainGrid.Children.Remove(FriendsButton);
                MainGrid.Children.Remove(ProfileButton);
            }
        }

        public void LoadProfileButton() {
            PlayerProfileName.Text = App.Current.Player.Nickname;
            PlayerProfileAvatar.Source = App.Current.GetPlayerAvatarImage();
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

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
        }
    }
}
