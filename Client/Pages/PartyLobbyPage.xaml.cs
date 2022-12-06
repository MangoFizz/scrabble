using Client.GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
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
    /// Interaction logic for PartyLobbyPage.xaml
    /// </summary>
    public partial class PartyLobbyPage : Page, IPartyManagerCallback, IPlayerManagerCallback {
        private PartyChatPage ChatPage = null;

        public List<Player> Friends { get; set; }

        public PartyLobbyPage() {
            InitializeComponent();
            ChatPage = new PartyChatPage();
            ChatFrame.Content = ChatPage;

            if(App.Current.CurrentParty == null) {
                App.Current.PartyManagerClient.CreateParty();
            }
            else {
                // Remove start game button if we are not the leader
                if(App.Current.CurrentParty.Leader.Nickname != App.Current.Player.Nickname) {
                    StartButton.Visibility = Visibility.Hidden;
                    StartButton.IsEnabled = false;
                }
            }

            // Add languages to combobox
            GameLanguageCombobox.Items.Add(Properties.Resources.COMMON_LANGUAGE_ENGLISH);
            GameLanguageCombobox.Items.Add(Properties.Resources.COMMON_LANGUAGE_SPANISH);
            GameLanguageCombobox.SelectedIndex = 1;
        }

        public void ReloadGroupList() {
            Action<Player, bool> addItem = (player, isFriend) => {
                bool isLeader = player.Nickname == App.Current.CurrentParty.Leader.Nickname;
                bool isSelf = player.Nickname == App.Current.Player.Nickname;

                Border border = new Border();
                border.Background = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
                border.BorderBrush = Brushes.White;
                border.BorderThickness = new Thickness(1);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Width = 416;

                Image image = new Image();
                var avatarPath = string.Format(Properties.Resources.PROFILE_AVATAR_FILE_PATH_FORMAT, player.Avatar);
                image.Source = new BitmapImage(new Uri(avatarPath, UriKind.Relative));
                image.Width = 60;
                image.Height = 60;
                stackPanel.Children.Add(image);

                StackPanel text = new StackPanel();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Margin = new Thickness(10, 0, 0, 0);
                text.Width = 230;

                Label nickname = new Label();
                nickname.Content = player.Nickname;
                nickname.Foreground = Brushes.White;
                nickname.FontSize = 18;
                nickname.FontWeight = FontWeights.Bold;
                nickname.Padding = new Thickness(0);
                text.Children.Add(nickname);

                Label status = new Label();
                status.FontSize = 18;
                status.Padding = new Thickness(0);
                status.Foreground = Brushes.White;

                if(isFriend) {
                    status.Content = Properties.Resources.FRIENDS_LIST_STATUS_ONLINE;
                }
                else if(isLeader) {
                    status.Content = Properties.Resources.PARTY_LOBBY_LEADER_LABEL;
                }

                text.Children.Add(status);

                stackPanel.Children.Add(text);

                StackPanel buttons = new StackPanel();
                buttons.Orientation = Orientation.Horizontal;
                buttons.HorizontalAlignment = HorizontalAlignment.Right;
                buttons.Visibility = Visibility.Hidden;
                buttons.Width = 80;

                border.MouseEnter += (sender, e) => {
                    buttons.Visibility = Visibility.Visible;
                };

                border.MouseLeave += (sender, e) => {
                    buttons.Visibility = Visibility.Hidden;
                };

                if(isFriend) {
                    Button invite = new Button();
                    invite.Content = "➕";
                    invite.Width = 30;
                    invite.Height = 30;
                    invite.Margin = new Thickness(50, 0, 0, 0);
                    invite.Click += (sender, e) => {
                        App.Current.PartyManagerClient.InvitePlayer(player);
                    };
                    buttons.Children.Add(invite);
                }
                else if(!isLeader && App.Current.Player.Nickname == App.Current.CurrentParty.Leader.Nickname) {
                    Button promote = new Button();
                    promote.Content = "👑";
                    promote.Width = 30;
                    promote.Height = 30;
                    promote.Margin = new Thickness(10, 0, 0, 0);
                    promote.Click += (sender, e) => {
                        App.Current.PartyManagerClient.TransferLeadership(player);
                    };
                    buttons.Children.Add(promote);
                    
                    Button kick = new Button();
                    kick.Content = "❌";
                    kick.Width = 30;
                    kick.Height = 30;
                    kick.Margin = new Thickness(10, 0, 0, 0);
                    kick.Click += (sender, e) => {
                        App.Current.PartyManagerClient.KickPlayer(player);
                    };
                    buttons.Children.Add(kick);
                }

                stackPanel.Children.Add(buttons);

                border.Child = stackPanel;

                FriendsListBox.Items.Add(border);
            };

            FriendListMessage.Visibility = Visibility.Hidden;

            FriendsListBox.Items.Clear();

            var party = App.Current.CurrentParty;

            if(party.Players != null) {
                foreach(var player in party.Players) {
                    addItem(player, false);
                }
            }

            if(Friends != null) {
                // Get online friends
                var onlineFriends = Friends.Where(f => f.status == PlayerStatus.Online).ToList();

                // Filter out players that are already in the party
                onlineFriends = onlineFriends.Where(f => {
                    if(f.Nickname == party.Leader.Nickname) {
                        return false;
                    }

                    if(party.Players != null) {
                        foreach(var player in party.Players) {
                            if(f.Nickname == player.Nickname) {
                                return false;
                            }
                        }
                    }

                    return true;
                }).ToList();

                if(onlineFriends.Count > 0) {
                    var friendsSubheader = new Label();
                    friendsSubheader.Content = Properties.Resources.PARTY_LOBBY_FRIENDS_LABEL;
                    friendsSubheader.Foreground = Brushes.White;
                    friendsSubheader.Focusable = false;
                    friendsSubheader.Padding = new Thickness(0, 5, 0, 5);

                    FriendsListBox.Items.Add(friendsSubheader);
                
                    foreach(var friend in onlineFriends) {
                        addItem(friend, true);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
            App.Current.CurrentParty = null;
            App.Current.PartyManagerClient.LeaveParty();
        }

        public void CreatePartyCallback(Party party) {
            ChatPage.PrintPlayerJoinMessage(App.Current.CurrentParty.Leader.Nickname);
            ReloadGroupList();
            App.Current.PlayerManagerClient.GetFriendList();
        }

        public void ReceiveInvitation(Player player, string partyId) {
            throw new NotImplementedException();
        }

        public void AcceptInvitationCallback(Party party) {
            throw new NotImplementedException();
        }

        public void ReceiveInvitationDecline(Player player) {
            throw new NotImplementedException();
        }

        public void ReceivePartyPlayerLeave(Player player) {
            ChatPage.PrintPlayerLeaveMessage(player.Nickname);
            ReloadGroupList();
            App.Current.PlayerManagerClient.GetFriendList();
        }

        public void ReceivePartyPlayerJoin(Player player) {
            ChatPage.PrintPlayerJoinMessage(player.Nickname);
            ReloadGroupList();
            App.Current.PlayerManagerClient.GetFriendList();
        }

        public void ReceiveGameStart() {
            NavigationService.Navigate(new PartyGamePage(ChatPage));
        }

        public void ReceivePartyKick(Player player) {
            throw new NotImplementedException();
        }

        public void ReceivePartyLeaderTransfer(Player player) {
            ChatPage.PrintPlayerIsLeaderMessage(player.Nickname);
            ReloadGroupList();
            App.Current.PlayerManagerClient.GetFriendList();

            // Show or hide start button if the player is the new leader
            bool playerIsLeader = App.Current.CurrentParty.Leader.Nickname == App.Current.Player.Nickname;
            StartButton.Visibility = playerIsLeader ? Visibility.Visible : Visibility.Hidden;
            StartButton.IsEnabled = playerIsLeader;
        }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult, Player player, string sessionId) {
            throw new NotImplementedException();
        }

        public void RegisterPlayerResponseHandler(PlayerManagerPlayerResgisterResult registrationResult) {
            throw new NotImplementedException();
        }

        public void SendFriendRequestResponseHandler(PlayerManagerPlayerFriendRequestResult result) {
            throw new NotImplementedException();
        }

        public void GetFriendListResponseHandler(Player[] friends) {
            Friends = friends.ToList();
            ReloadGroupList();
        }

        public void GetFriendRequestsResponseHandler(Player[] friendRequests) {
            throw new NotImplementedException();
        }

        public void ReceiveFriendRequest(Player player) {
            throw new NotImplementedException();
        }

        public void ReceiveFriendAdd(Player player) {
            App.Current.PlayerManagerClient.GetFriendList();
            ReloadGroupList();
        }

        public void FriendConnect(Player player) {
            App.Current.PlayerManagerClient.GetFriendList();
            ReloadGroupList();
        }

        public void FriendDisconnect(Player player) {
            var friend = Friends.Find(p => p.Nickname == player.Nickname);
            Friends.Remove(friend);
            ReloadGroupList();
        }

        private void GameTimeLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(GameTimeLimitIndicator != null) {
                GameTimeLimitIndicator.Content = GameTimeLimitSlider.Value.ToString() + "m";
            } 
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PartyManagerClient.StartGame((GameSupportedLanguage)GameLanguageCombobox.SelectedIndex, (int)GameTimeLimitSlider.Value);
        }

        public void StartGameCallback(GameStartResult result) {
            switch(result) {
                case GameStartResult.NotEnoughPlayers:
                    ResultText.Content = Properties.Resources.PARTY_LOBBY_NOT_ENOUGH_PLAYERS;
                    break;

                case GameStartResult.Success:
                    ResultText.Content = Properties.Resources.PARTY_LOBBY_GAME_START_SUCCESS;
                    break;

                default:
                    break;
            }
        }
    }
}
