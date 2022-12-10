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
    /// Interaction logic for PlayerProfilePage.xaml
    /// </summary>
    public partial class PlayerProfilePage : Page {
        public GameService.Player player { get; set; }

        public PlayerProfilePage(GameService.Player player) {
            InitializeComponent();
            this.player = player;
            LoadProfileData();
        }

        private void LoadProfileData() {
            ProfileNickname.Content = player.Nickname;
            ProfileEmail.Content = player.Email;
            ProfileAvatar.Source = App.Current.GetPlayerAvatarImage();
            ProfileGamesCount.Content = player.GamesCount;
            ProfileWinsCount.Content = player.WinsCount;
            ProfileRegisteredDate.Content = player.Registered.ToString("dd/MM/yyyy");
        }

        private void RectagleMouseLeft_ButtonUp(object sender, MouseButtonEventArgs e) {
            App.Current.CloseSecondaryPanel();
        }
    }
}
