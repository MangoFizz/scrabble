using Client.GameService;
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
    /// Interaction logic for FriendsListPage.xaml
    /// </summary>
    public partial class FriendsListPage : Page {
        public List<Player> FriendList = new List<Player>();
        public List<Player> FriendRequests = new List<Player>();

        private void LoadFriendList() {
            App.Current.PlayerManagerClient.GetFriendList();
            App.Current.PlayerManagerClient.GetFriendRequests();
        }

        public void RefreshFriendList() {
            FriendsListBox.Items.Clear();

            foreach(var friend in FriendList) {
                FriendsListBox.Items.Add(friend);
            }

            foreach(var friendRequest in FriendRequests) {
                FriendsListBox.Items.Add(friendRequest);
            }

            if(FriendsListBox.Items.Count > 0) {
                ListMessage.Content = "";
            }


        }

        public FriendsListPage() {
            InitializeComponent();
            LoadFriendList();
        }

        private void RectagleMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            App.Current.CloseFriendsList();
        }

        private void AddFriendButtonClick(object sender, RoutedEventArgs e) {
            if(NicknameTextBox.Text.Length > 0) {
                App.Current.PlayerManagerClient.SendFriendRequest(NicknameTextBox.Text);
            }
        }
    }
}
