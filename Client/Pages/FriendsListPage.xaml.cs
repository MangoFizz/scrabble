using Client.GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    public partial class FriendsListPage : Page {
        public List<Player> Friends = new List<Player>();
        public List<Player> FriendRequests = new List<Player>();

        public void RequestFriendsList() {
            App.Current.PlayerManagerClient.GetFriendRequests();
            App.Current.PlayerManagerClient.GetFriendList();
        }

        public void RefreshFriendList() {
            FriendsListBox.Items.Clear();

            if(FriendRequests != null) {
                foreach(var friendRequest in FriendRequests) {
                    AddFriendListEntry(friendRequest, true);
                }
            }
            
            if(Friends != null) {
                foreach(var friend in Friends) {
                    AddFriendListEntry(friend, false);
                }
            }

            if(FriendsListBox.Items.Count > 0) {
                ListMessage.Content = "";
            }
            else {
                ListMessage.Content = Properties.Resources.COMMON_EMPTY;
            }
        }

        public FriendsListPage() {
            InitializeComponent();
            RequestFriendsList();
        }

        private void CloseRectangle_ButtonUp(object sender, MouseButtonEventArgs e) {
            SetCloseAnimationEndTimer();
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e) {
            if(NicknameTextBox.Text.Length > 0) {
                App.Current.PlayerManagerClient.SendFriendRequest(NicknameTextBox.Text);
            }
        }

        private StackPanel GetFriendListEntryButtons(Player player) {
            StackPanel entryButtonsContainer = new StackPanel();
            entryButtonsContainer.Orientation = Orientation.Horizontal;
            entryButtonsContainer.HorizontalAlignment = HorizontalAlignment.Right;

            Button acceptButton = new Button();
            acceptButton.Content = "✔";
            acceptButton.Width = 30;
            acceptButton.Height = 30;
            acceptButton.Margin = new Thickness(10, 0, 0, 0);
            acceptButton.Click += (sender, e) => {
                App.Current.PlayerManagerClient.AcceptFriendRequest(player.Nickname);
            };
            entryButtonsContainer.Children.Add(acceptButton);

            Button declineButton = new Button();
            declineButton.Content = "❌";
            declineButton.Width = 30;
            declineButton.Height = 30;
            declineButton.Margin = new Thickness(10, 0, 0, 0);
            declineButton.Click += (sender, e) => {
                App.Current.PlayerManagerClient.DeclineFriendRequest(player.Nickname);
            };
            entryButtonsContainer.Children.Add(declineButton);

            return entryButtonsContainer;
        }

        private void SetFriendStatusLabel(Player.StatusType status, Label statusLabel) {
            switch(status) {
                case Player.StatusType.Online:
                    statusLabel.Content = Properties.Resources.FRIENDS_LIST_STATUS_ONLINE;
                    statusLabel.Foreground = Brushes.Green;
                    break;
                case Player.StatusType.Offline:
                    statusLabel.Content = Properties.Resources.FRIENDS_LIST_STATUS_OFFLINE;
                    statusLabel.Foreground = Brushes.Gray;
                    break;
                case Player.StatusType.InGame:
                    statusLabel.Content = Properties.Resources.FRIENDS_LIST_STATUS_IN_GAME;
                    statusLabel.Foreground = Brushes.Blue;
                    break;
            }
        }

        private Border GetFriendListEntry(Player player, bool pending) {
            Border entryBorder = new Border();
            entryBorder.BorderBrush = Brushes.Gray;
            entryBorder.BorderThickness = new Thickness(1);

            StackPanel entryContainer = new StackPanel();
            entryContainer.Orientation = Orientation.Horizontal;
            entryContainer.Width = 325;

            Image avatarImage = new Image();
            avatarImage.Source = App.Current.GetPlayerAvatarImage(player);
            avatarImage.Width = 60;
            avatarImage.Height = 60;
            entryContainer.Children.Add(avatarImage);

            StackPanel textContainer = new StackPanel();
            textContainer.VerticalAlignment = VerticalAlignment.Center;
            textContainer.Margin = new Thickness(10, 0, 0, 0);
            textContainer.Width = 160;

            Label friendNicknameLabel = new Label();
            friendNicknameLabel.Content = player.Nickname;
            friendNicknameLabel.FontSize = 18;
            friendNicknameLabel.FontWeight = FontWeights.Bold;
            friendNicknameLabel.Padding = new Thickness(0);
            textContainer.Children.Add(friendNicknameLabel);

            Label friendStatusLabel = new Label();
            friendStatusLabel.FontSize = 18;
            friendStatusLabel.Padding = new Thickness(0);
            if(pending) {
                friendStatusLabel.Content = Properties.Resources.FRIENDS_LIST_STATUS_PENDING;
            }
            else {
                SetFriendStatusLabel(player.Status, friendStatusLabel);
            }

            textContainer.Children.Add(friendStatusLabel);

            entryContainer.Children.Add(textContainer);

            if(pending) {
                var buttons = GetFriendListEntryButtons(player);
                entryContainer.Children.Add(buttons);
            }

            entryBorder.Child = entryContainer;

            return entryBorder;
        }

        private void AddFriendListEntry(Player player, bool pending) {
            var friendListEntry = GetFriendListEntry(player, pending);
            FriendsListBox.Items.Add(friendListEntry);
        }

        private void SetCloseAnimationEndTimer() {
            Task.Factory.StartNew(() => {
                Thread.Sleep(175);
                Dispatcher.Invoke(() => {
                    App.Current.CloseFriendsList();
                });
            });
        }
    }

    partial class FriendsListPage : IPlayerManagerCallback {
        public void SendFriendRequestResponseHandler(GameService.PlayerManagerPlayerFriendRequestResult result) {
            switch(result) {
                case PlayerManagerPlayerFriendRequestResult.Success:
                    FriendRequestResultMessage.Content = Properties.Resources.FRIENDS_LIST_FRIEND_REQUEST_SUCCESS;
                    FriendRequestResultMessage.Foreground = Brushes.Green;
                    NicknameTextBox.Text = "";
                    break;

                case PlayerManagerPlayerFriendRequestResult.SelfRequest:
                    FriendRequestResultMessage.Content = Properties.Resources.FRIENDS_LIST_FRIEND_REQUEST_AUTOREQUEST;
                    FriendRequestResultMessage.Foreground = Brushes.Red;
                    NicknameTextBox.Text = "";
                    break;

                case PlayerManagerPlayerFriendRequestResult.PendingRequest:
                    FriendRequestResultMessage.Content = Properties.Resources.FRIENDS_LIST_FRIEND_REQUEST_PENDING;
                    FriendRequestResultMessage.Foreground = Brushes.Red;
                    NicknameTextBox.Text = "";
                    break;

                case PlayerManagerPlayerFriendRequestResult.AlreadyFriends:
                    FriendRequestResultMessage.Content = Properties.Resources.FRIENDS_LIST_FRIEND_REQUEST_ALREADY_FRIENDS;
                    FriendRequestResultMessage.Foreground = Brushes.Red;
                    NicknameTextBox.Text = "";
                    break;

                case PlayerManagerPlayerFriendRequestResult.ReceiverPlayerDoesNotExists:
                    FriendRequestResultMessage.Content = Properties.Resources.FRIENDS_LIST_FRIEND_REQUEST_PLAYER_DOES_NOT_EXISTS;
                    FriendRequestResultMessage.Foreground = Brushes.Red;
                    break;

                default:
                    FriendRequestResultMessage.Content = Properties.Resources.COMMON_UNKNOWN_ERROR;
                    FriendRequestResultMessage.Foreground = Brushes.Red;
                    break;
            }
        }

        public void ReceiveFriendAdd(Player player) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            if(FriendRequests == null) {
                FriendRequests = new List<Player>();
            }

            var friendRequest = FriendRequests.Find(x => x.Nickname == player.Nickname);
            if(friendRequest != null) {
                FriendRequests.Remove(friendRequest);
            }

            Friends.Add(player);
            RefreshFriendList();
        }

        public void ReceiveFriendRequest(Player player) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            if(FriendRequests == null) {
                FriendRequests = new List<Player>();
            }

            var friendRequest = FriendRequests.Find(x => x.Nickname == player.Nickname);
            if(friendRequest == null) {
                FriendRequests.Add(player);
            }

            RefreshFriendList();
        }

        public void UpdateFriendStatus(Player friend, Player.StatusType status) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            var localFriend = Friends.Find(x => x.Nickname == friend.Nickname);
            if(localFriend != null) {
                localFriend.Status = status;
            }

            RefreshFriendList();
        }

        public void Disconnect(DisconnectionReason reason) {
            throw new NotImplementedException();
        }

        public void GetFriendListResponseHandler(Player[] friends) {
            throw new NotImplementedException();
        }

        public void GetFriendRequestsResponseHandler(Player[] friendRequests) {
            throw new NotImplementedException();
        }

        public void LoginResponseHandler(PlayerManagerPlayerAuthResult loginResult, Player player, string sessionId) {
            throw new NotImplementedException();
        }

        public void RegisterPlayerResponseHandler(PlayerManagerPlayerResgisterResult registrationResult) {
            throw new NotImplementedException();
        }

        public void VerificationResponseHandler(PlayerManagerPlayerVerificationResult verificationResult) {
            throw new NotImplementedException();
        }

        public void UpdatePlayerAvatarCallback(short avatarId) {
            throw new NotImplementedException();
        }
    }
}
