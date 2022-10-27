using Client.GameChatService;
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
    public partial class Chat : Page, IGameChatCallback {
        private GameChatClient chatClient;
        private List<ChatUser> chatUsers;

        public Chat() {
            InitializeComponent();

            var context = new InstanceContext(this);
            chatClient = new GameChatClient(context);
            chatClient.join(MainWindow.username);
        }

        private void sendMessage() {
            if(this.messageInput.Text.Length == 0) {
                return;
            }

            // Send message to server
            this.chatClient.say(this.messageInput.Text);
            this.messageInput.Text = "";

            this.updateChatScroll();
        }

        private void updateChatScroll() {
            if(this.chat.Items.Count == 0) {
                return;
            }

            // Move scroll to bottom
            this.chat.ScrollIntoView(this.chat.Items[this.chat.Items.Count - 1]);
        }

        private void updateUsersList() {
            this.chatUsersListBox.Items.Clear();
            foreach(ChatUser user in this.chatUsers) {
                this.chatUsersListBox.Items.Add(user.username);
            }
        }

        public void Receive(ChatUser sender, string message) {
            var item = new ListBoxItem();
            item.Content = sender.username + ": " + message;
            chat.Items.Add(item);

            this.updateChatScroll();
        }

        public void ReceiveWhisper(ChatUser sender, string message) {
            var item = new ListBoxItem();
            item.Content = sender.username + ": " + message;
            chat.Items.Add(item);

            this.updateChatScroll();
        }

        public void UserEnter(ChatUser person) {
            chatUsers.Add(person);

            var item = new ListBoxItem();
            item.Content = String.Format(Properties.Resources.userJoinChatMessageFormat, person.username);
            chat.Items.Add(item);

            this.updateChatScroll();
            this.updateUsersList();
        }

        public void UserJoinResponse(ChatUser[] users) {
            chatUsers = new List<ChatUser>(users);
            this.updateUsersList();
        }

        public void UserLeave(ChatUser person) {
            var user = chatUsers.Find(u => u.username == person.username);
            chatUsers.Remove(user);

            var item = new ListBoxItem();
            item.Content = String.Format(Properties.Resources.userLeaveChatMessageFormat, person.username);
            chat.Items.Add(item);

            this.updateChatScroll();
            this.updateUsersList();
        }

        private void SendMessageButton(object sender, RoutedEventArgs e) {
            sendMessage();
        }

        private void onChatInputKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Return) {
                sendMessage();
            }
        }

        private void onBackButtonClick(object sender, RoutedEventArgs e) {
            this.chatClient.leave();
            this.NavigationService.GoBack();
        }
    }
}
