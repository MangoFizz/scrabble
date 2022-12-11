using Client.GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Client {
    /// <summary>
    /// Interaction logic for PartyLobbyPage.xaml
    /// </summary>
    public partial class PartyLobbyPage : Page {
        private PartyChatPage ChatPage = null;

        public List<Player> Friends { get; set; }

        private void SetUpLanguageCombobox() {
            GameLanguageCombobox.Items.Add(Properties.Resources.COMMON_LANGUAGE_ENGLISH);
            GameLanguageCombobox.Items.Add(Properties.Resources.COMMON_LANGUAGE_SPANISH);
            GameLanguageCombobox.SelectedIndex = 1;
        }

        private void HideResultMessage() {
            ResultText.Visibility = Visibility.Hidden;
        }

        private void InitializeParty() {
            if(App.Current.CurrentParty == null) {
                App.Current.CreateParty();
            }
            if(App.Current.CurrentParty.Leader.Nickname != App.Current.Player.Nickname) {
                StartButton.Visibility = Visibility.Hidden;
                StartButton.IsEnabled = false;
                GameTimeLimitSlider.IsEnabled = false;
                GameLanguageCombobox.IsEnabled = false;
            }

            ChatPage.PrintPlayerJoinMessage(App.Current.CurrentParty.Leader.Nickname);
            SetLobbyCode();
            ReloadGroupList();
            App.Current.PlayerManagerClient.GetFriendList();
        }

        private void SetUpChatFrame() {
            ChatPage = new PartyChatPage();
            ChatFrame.Content = ChatPage;
        }

        private void SetLobbyCode() {
            LobbyCode.Content = App.Current.CurrentParty.InviteCode;
        }

        private StackPanel GetGroupListEntryButtons(Player player, bool isFriend) {
            StackPanel entryButtonsContainer = new StackPanel();
            entryButtonsContainer.Orientation = Orientation.Horizontal;
            entryButtonsContainer.HorizontalAlignment = HorizontalAlignment.Right;
            entryButtonsContainer.Visibility = Visibility.Hidden;
            entryButtonsContainer.Width = 80;

            if(App.Current.Player.Nickname == App.Current.CurrentParty.Leader.Nickname) {
                if(isFriend) {
                    Button inviteButton = new Button();
                    inviteButton.Content = "➕";
                    inviteButton.Width = 30;
                    inviteButton.Height = 30;
                    inviteButton.Margin = new Thickness(50, 0, 0, 0);
                    inviteButton.Click += (sender, e) => {
                        App.Current.PartyManagerClient.InviteFriend(player);
                    };
                    entryButtonsContainer.Children.Add(inviteButton);
                }
                else if(player.Nickname != App.Current.CurrentParty.Leader.Nickname) {
                    Button promoteButton = new Button();
                    promoteButton.Content = "👑";
                    promoteButton.Width = 30;
                    promoteButton.Height = 30;
                    promoteButton.Margin = new Thickness(10, 0, 0, 0);
                    promoteButton.Click += (sender, e) => {
                        App.Current.PartyManagerClient.TransferLeadership(player);
                    };
                    entryButtonsContainer.Children.Add(promoteButton);

                    Button kickButton = new Button();
                    kickButton.Content = "❌";
                    kickButton.Width = 30;
                    kickButton.Height = 30;
                    kickButton.Margin = new Thickness(10, 0, 0, 0);
                    kickButton.Click += (sender, e) => {
                        App.Current.PartyManagerClient.KickPlayer(player);
                    };
                    entryButtonsContainer.Children.Add(kickButton);
                }
            }

            return entryButtonsContainer;
        }

        private Border GetGroupListEntry(Player player, bool isFriend) {
            Border entryBorder = new Border();
            entryBorder.Background = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
            entryBorder.BorderBrush = Brushes.White;
            entryBorder.BorderThickness = new Thickness(1);

            StackPanel entryContainer = new StackPanel();
            entryContainer.Orientation = Orientation.Horizontal;
            entryContainer.Width = 416;

            Image avatarImage = new Image();
            avatarImage.Source = App.Current.GetPlayerAvatarImage(player);
            avatarImage.Width = 60;
            avatarImage.Height = 60;
            entryContainer.Children.Add(avatarImage);

            StackPanel textContainer = new StackPanel();
            textContainer.VerticalAlignment = VerticalAlignment.Center;
            textContainer.Margin = new Thickness(10, 0, 0, 0);
            textContainer.Width = 230;

            Label friendNicknameLabel = new Label();
            friendNicknameLabel.Content = player.Nickname;
            friendNicknameLabel.Foreground = Brushes.White;
            friendNicknameLabel.FontSize = 18;
            friendNicknameLabel.FontWeight = FontWeights.Bold;
            friendNicknameLabel.Padding = new Thickness(0);
            textContainer.Children.Add(friendNicknameLabel);

            Label friendStatusLabel = new Label();
            friendStatusLabel.FontSize = 18;
            friendStatusLabel.Padding = new Thickness(0);
            friendStatusLabel.Foreground = Brushes.White;
            if(isFriend) {
                friendStatusLabel.Content = Properties.Resources.FRIENDS_LIST_STATUS_ONLINE;
            }
            else if(player.Nickname == App.Current.CurrentParty.Leader.Nickname) {
                friendStatusLabel.Content = Properties.Resources.PARTY_LOBBY_LEADER_LABEL;
            }

            textContainer.Children.Add(friendStatusLabel);
            entryContainer.Children.Add(textContainer);

            var buttonsContainer = GetGroupListEntryButtons(player, isFriend);
            entryContainer.Children.Add(buttonsContainer);

            entryBorder.MouseEnter += (sender, e) => {
                buttonsContainer.Visibility = Visibility.Visible;
            };
            
            entryBorder.MouseLeave += (sender, e) => {
                buttonsContainer.Visibility = Visibility.Hidden;
            };
            
            entryBorder.Child = entryContainer;

            return entryBorder;
        }

        private void AddGroupListEntry(Player player, bool isFriend) {
            Border border = GetGroupListEntry(player, isFriend);
            FriendsListBox.Items.Add(border);
        }

        public PartyLobbyPage() {
            InitializeComponent();
            
            SetUpChatFrame();
            HideResultMessage();
            InitializeParty();
            SetUpLanguageCombobox();
        }


        public void ReloadGroupList() {
            FriendListMessage.Visibility = Visibility.Hidden;
            FriendsListBox.Items.Clear();

            var party = App.Current.CurrentParty;
            if(party.Players != null) {
                foreach(var player in party.Players) {
                    AddGroupListEntry(player, false);
                }
            }

            if(Friends != null) {
                var onlineFriends = Friends.Where(f => f.Status == Player.StatusType.Online).ToList();

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
                        AddGroupListEntry(friend, true);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
            App.Current.CurrentParty = null;
            App.Current.PartyManagerClient.LeaveParty();
        }

        private void GameTimeLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(GameTimeLimitIndicator != null) {
                GameTimeLimitIndicator.Content = GameTimeLimitSlider.Value.ToString() + "m";
                if(App.Current.CurrentParty != null && App.Current.Player.Nickname == App.Current.CurrentParty.Leader.Nickname) {
                    App.Current.PartyManagerClient.UpdateTimeLimitSetting((int)GameTimeLimitSlider.Value);
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            App.Current.PartyManagerClient.StartGame((GameSupportedLanguage)GameLanguageCombobox.SelectedIndex, (int)GameTimeLimitSlider.Value);
        }
    }

    public partial class PartyLobbyPage : IPlayerManagerCallback {
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

        public void UpdateFriendStatus(Player friend, Player.StatusType status) {
            App.Current.PlayerManagerClient.GetFriendList();
            ReloadGroupList();
        }

        public void Disconnect(DisconnectionReason reason) {
            throw new NotImplementedException();
        }

        public void VerificationResponseHandler(PlayerManagerPlayerVerificationResult verificationResult) {
            throw new NotImplementedException();
        }

        public void UpdatePlayerAvatarCallback(short newAvatarId) {
            throw new NotImplementedException();
        }
    }

    public partial class PartyLobbyPage : IPartyManagerCallback {
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
            GameTimeLimitSlider.IsEnabled = playerIsLeader;
            GameLanguageCombobox.IsEnabled = playerIsLeader;
        }

        public void StartGameCallback(GameStartResult result) {
            ResultText.Visibility = Visibility.Visible;
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

        public void ReceivePartyTimeLimitUpdate(int time) {
            if(GameTimeLimitIndicator != null) {
                GameTimeLimitIndicator.Content = time.ToString() + "m";
                GameTimeLimitSlider.Value = time;
            }
        }

        public void ReceivePartyLanguageUpdate(GameSupportedLanguage language) {
            if(GameLanguageCombobox != null) {
                GameLanguageCombobox.SelectedIndex = (int)language;
            }
        }

        private void GameLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(App.Current.CurrentParty != null && App.Current.Player.Nickname == App.Current.CurrentParty.Leader.Nickname) {
                App.Current.PartyManagerClient.UpdateLanguageSetting((GameSupportedLanguage)GameLanguageCombobox.SelectedIndex);
            }
        }

        public void JoinPartyCallback(JoinPartyResult result, Party party) {
            throw new NotImplementedException();
        }
    }
}
