﻿using Client.GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Interaction logic for PartyChatPage.xaml
    /// </summary>
    public partial class PartyChatPage : Page, IPartyChatCallback {
        private PartyChatClient _PartyChatClient = null;

        public PartyChatClient PartyChatClient {
            get {
                if(_PartyChatClient == null) {
                    var context = new InstanceContext(this);
                    var service = new PartyChatClient(context);
                    _PartyChatClient = service;
                }
                return _PartyChatClient;
            }
        }

        public PartyChatPage() {
            InitializeComponent();
            Connect();
        }

        private void Connect() {
            PartyChatClient.ConnectPartyChat(App.Current.SessionId);
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs eventArgs) {
            MessageInputWatermark.Visibility = Visibility.Hidden;
            if(eventArgs.Key == Key.Return) {
                SendMessage();
            }
        }

        private void MessageInput_KeyUp(object sender, KeyEventArgs e) {
            if(MessageInput.Text.Length == 0) {
                MessageInputWatermark.Visibility = Visibility.Visible;
            }
            else {
                MessageInputWatermark.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateChatScroll() {
            if(this.ChatListBox.Items.Count == 0) {
                return;
            }
            this.ChatListBox.ScrollIntoView(this.ChatListBox.Items[this.ChatListBox.Items.Count - 1]);
        }

        public void Print(string message) {
            var chatEntry = new ListBoxItem();
            chatEntry.Content = message;
            chatEntry.Foreground = Brushes.White;
            chatEntry.FontSize = 20;
            ChatListBox.Items.Add(chatEntry);
            UpdateChatScroll();
        }

        public void Receive(Player sender, string message) {
            Print(string.Format(Properties.Resources.PARTY_CHAT_MESSAGE_FORMAT, sender.Nickname, message));
        }

        public void PrintPlayerJoinMessage(string nickname) {
            Print(string.Format(Properties.Resources.PARTY_CHAT_PLAYER_JOIN_MESSAGE_FORMAT, nickname));
        }
        
        public void PrintPlayerLeaveMessage(string nickname) {
            Print(string.Format(Properties.Resources.PARTY_CHAT_PLAYER_LEAVE_MESSAGE_FORMAT, nickname));
        }

        public void PrintPlayerIsLeaderMessage(string nickname) {
            Print(string.Format(Properties.Resources.PARTY_CHAT_LEADER_TRANSFER_MESSAGE_FORMAT, nickname));
        }
        
        private void SendMessage() {
            if(MessageInput.Text.Length == 0) {
                return;
            }
            PartyChatClient.Say(MessageInput.Text);
            MessageInput.Text = "";
        }
    }
}
