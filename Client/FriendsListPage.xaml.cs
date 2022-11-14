﻿using Client.GameService;
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
    public partial class FriendsListPage : Page {
        public List<Player> FriendList = new List<Player>();
        public List<Player> FriendRequests = new List<Player>();

        private void LoadFriendList() {
            App.Current.PlayerManagerClient.GetFriendList();
            App.Current.PlayerManagerClient.GetFriendRequests();
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
                image.Source = new BitmapImage(new Uri($"/Assets/images/avatars/default_{player.Avatar}.png", UriKind.Relative));
                image.Width = 60;
                image.Height = 60;
                stackPanel.Children.Add(image);

                StackPanel text = new StackPanel();
                text.VerticalAlignment = VerticalAlignment.Center;
                text.Margin = new Thickness(10, 0, 0, 0);
                text.Width = 160;

                Label nickname = new Label();
                nickname.Content = player.Nickname;
                nickname.FontSize = 16;
                nickname.FontWeight = FontWeights.Bold;
                nickname.Padding = new Thickness(0);
                text.Children.Add(nickname);

                Label status = new Label();
                status.FontSize = 14;
                status.Padding = new Thickness(0);
                
                if(pending) {
                    status.Content = "Pending";
                }
                else {
                    status.Content = "Offline";
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

            foreach(var friendRequest in FriendRequests) {
                addItem(friendRequest, true);
            }

            foreach(var friend in FriendList) {
                addItem(friend, false);
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
