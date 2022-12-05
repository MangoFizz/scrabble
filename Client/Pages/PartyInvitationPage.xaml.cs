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
    /// Interaction logic for InviteNotificationPage.xaml
    /// </summary>
    public partial class PartyInvitationPage : Page {
        public Player Player { get; set; }

        public PartyInvitationPage(Player player) {
            InitializeComponent();
            Player = player;
            InviteMessage.Text = string.Format(Properties.Resources.PARTY_INVITATION_TEXT, player.Nickname);
            var avatarPath = string.Format(Properties.Resources.PROFILE_AVATAR_FILE_PATH_FORMAT, player.Avatar);
            PlayerAvatar.Source = new BitmapImage(new Uri(avatarPath, UriKind.Relative));
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PartyManagerClient.AcceptInvitation(Player);
        }
    }
}
