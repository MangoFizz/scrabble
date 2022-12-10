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

            Action<Player, bool> addItem = (player, pending) => {
                Border border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new Thickness(1);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Width = 325;

                Image image = new Image();
                image.Source = App.Current.GetPlayerAvatarImage(player);
                image.Width = 60;
                image.Height = 60;
                stackPanel.Children.Add(image);

                StackPanel text = new StackPanel();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Margin = new Thickness(10, 0, 0, 0);
                text.Width = 160;

                Label nickname = new Label();
                nickname.Content = player.Nickname;
                nickname.FontSize = 18;
                nickname.FontWeight = FontWeights.Bold;
                nickname.Padding = new Thickness(0);
                text.Children.Add(nickname);

                Label status = new Label();
                status.FontSize = 18;
                status.Padding = new Thickness(0);
                
                if(pending) {
                    status.Content = Properties.Resources.FRIENDS_LIST_STATUS_PENDING;
                }
                else {
                    switch(player.Status) {
                        case Player.StatusType.Online:
                            status.Content = Properties.Resources.FRIENDS_LIST_STATUS_ONLINE;
                            status.Foreground = Brushes.Green;
                            break;
                        case Player.StatusType.Offline:
                            status.Content = Properties.Resources.FRIENDS_LIST_STATUS_OFFLINE;
                            status.Foreground = Brushes.Gray;
                            break;
                        case Player.StatusType.InGame:
                            status.Content = Properties.Resources.FRIENDS_LIST_STATUS_IN_GAME;
                            status.Foreground = Brushes.Blue;
                            break;
                    }
                }

                text.Children.Add(status);

                stackPanel.Children.Add(text);

                if(pending) {
                    StackPanel buttons = new StackPanel();
                    buttons.Orientation = Orientation.Horizontal;
                    buttons.HorizontalAlignment = HorizontalAlignment.Right;

                    Button accept = new Button();
                    accept.Content = "✔";
                    accept.Width = 30;
                    accept.Height = 30;
                    accept.Margin = new Thickness(10, 0, 0, 0);
                    accept.Click += (sender, e) => {
                        App.Current.PlayerManagerClient.AcceptFriendRequest(player.Nickname);
                    };
                    buttons.Children.Add(accept);

                    Button decline = new Button();
                    decline.Content = "❌";
                    decline.Width = 30;
                    decline.Height = 30;
                    decline.Margin = new Thickness(10, 0, 0, 0);
                    decline.Click += (sender, e) => {
                        App.Current.PlayerManagerClient.DeclineFriendRequest(player.Nickname);
                    };
                    buttons.Children.Add(decline);

                    stackPanel.Children.Add(buttons);
                }

                border.Child = stackPanel;

                FriendsListBox.Items.Add(border);
            };

            if(FriendRequests != null) {
                foreach(var friendRequest in FriendRequests) {
                    addItem(friendRequest, true);
                }
            }
            
            if(Friends != null) {
                foreach(var friend in Friends) {
                    addItem(friend, false);
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

        private void RectagleMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Task.Factory.StartNew(() => {
                Thread.Sleep(175);
                Dispatcher.Invoke(() => {
                    App.Current.CloseFriendsList();
                });
            });
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e) {
            if(NicknameTextBox.Text.Length > 0) {
                App.Current.PlayerManagerClient.SendFriendRequest(NicknameTextBox.Text);
            }
        }
        
        public void SendFriendRequestResponseHandler(GameService.PlayerManagerPlayerFriendRequestResult result) {
            switch(result) {
                case GameService.PlayerManagerPlayerFriendRequestResult.Success: 
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

        public void FriendAddHandler(Player friend) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            if(FriendRequests == null) {
                FriendRequests = new List<Player>();
            }

            var friendRequest = FriendRequests.Find(x => x.Nickname == friend.Nickname);
            if(friendRequest != null) {
                FriendRequests.Remove(friendRequest);
            }

            Friends.Add(friend);
            RefreshFriendList();
        }

        public void FriendRequestReceiveHandler(Player friend) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            if(FriendRequests == null) {
                FriendRequests = new List<Player>();
            }

            var friendRequest = FriendRequests.Find(x => x.Nickname == friend.Nickname);
            if(friendRequest == null) {
                FriendRequests.Add(friend);
            }

            RefreshFriendList();
        }

        public void UpdateFriendStatus(Player player, Player.StatusType status) {
            if(Friends == null) {
                Friends = new List<Player>();
            }

            var friend = Friends.Find(x => x.Nickname == player.Nickname);
            if(friend != null) {
                friend.Status = status;
            }

            RefreshFriendList();
        }
    }
}
