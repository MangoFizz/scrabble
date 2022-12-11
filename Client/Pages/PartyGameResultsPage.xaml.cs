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

namespace Client.Pages {
    /// <summary>
    /// Interaction logic for PartyGameResultsPage.xaml
    /// </summary>
    public partial class PartyGameResultsPage : Page {
        private void SetPlacementHeader() {
            var placement = App.Current.CurrentParty.Players.OrderByDescending(p => p.Score).ToList();
            var player = placement.Find(p => p.Nickname == App.Current.Player.Nickname);
            var playerPlacement = placement.IndexOf(player) + 1;

            switch(playerPlacement) {
                case 1:
                    PlayerPlacement.Content = Properties.Resources.PARTY_GAME_RESULTS_MENU_FIRST_PLACE_HEADER;
                    break;
                case 2:
                    PlayerPlacement.Content = Properties.Resources.PARTY_GAME_RESULTS_MENU_SECOND_PLACE_HEADER;
                    break;
                case 3:
                    PlayerPlacement.Content = Properties.Resources.PARTY_GAME_RESULTS_MENU_THIRD_PLACE_HEADER;
                    break;
                default:
                    PlayerPlacement.Content = Properties.Resources.PARTY_GAME_RESULTS_MENU_FOURTH_PLACE_HEADER;
                    break;
            }
        }
        
        private void SetUpPlayersScore() {
            var party = App.Current.CurrentParty;
            var players = party.Players;

            Player1Avatar.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{players[0].Avatar}.png"));
            Player1Nickname.Content = players[0].Nickname;
            Player1Score.Content = $"{players[0].Score} pts.";

            if(players.Length >= 2) {
                Player2Avatar.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{players[1].Avatar}.png"));
                Player2Nickname.Content = players[1].Nickname;
                Player2Score.Content = $"{players[1].Score} pts.";
            }
            else {
                Player2Entry.Visibility = Visibility.Hidden;
            }

            if(players.Length >= 3) {
                Player3Avatar.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{players[2].Avatar}.png"));
                Player3Nickname.Content = players[2].Nickname;
                Player3Score.Content = $"{players[2].Score} pts.";
            }
            else {
                Player3Entry.Visibility = Visibility.Hidden;
            }

            if(players.Length >= 4) {
                Player4Avatar.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{players[3].Avatar}.png"));
                Player4Nickname.Content = players[3].Nickname;
                Player4Score.Content = $"{players[3].Score} pts.";
            }
            else {
                Player4Entry.Visibility = Visibility.Hidden;
            }
        }

        public PartyGameResultsPage() {
            InitializeComponent();
            SetPlacementHeader();
            SetUpPlayersScore();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            App.Current.MainFrame.GoBack();
            App.Current.MainFrame.Navigate(new PartyLobbyPage());
            App.Current.MainFrame.RemoveBackEntry();
            App.Current.CloseSecondaryPanel();
        }
    }
}
