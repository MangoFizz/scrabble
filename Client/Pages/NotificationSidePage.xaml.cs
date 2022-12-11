using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    /// <summary>
    /// Interaction logic for NotificationSidePage.xaml
    /// </summary>
    public partial class NotificationSidePage : Page {
        public NotificationSidePage() {
            InitializeComponent();
        }

        private bool InviteNotificationAlreadyShown(string invideSenderNickname) {
            foreach(var child in NotificationsStackPanel.Children) {
                if(typeof(Frame).IsInstanceOfType(child)) {
                    var childFrame = (Frame)child;
                    if(typeof(PartyInvitationPage).IsInstanceOfType(childFrame.Content)) {
                        var invitationPage = (PartyInvitationPage)childFrame.Content;
                        if(invitationPage.Player.Nickname == invideSenderNickname) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void SetNotificationEntryCloseTimer(Frame notificationFrame) {
            Task.Factory.StartNew(() => {
                Thread.Sleep(5600);
                Dispatcher.Invoke(() => {
                    NotificationsStackPanel.Children.Remove(notificationFrame);
                });
            });
        }

        private void AddInviteNotificationFrame(PartyInvitationPage page) {
            page.VerticalAlignment = VerticalAlignment.Top;
            var inviteFrame = new Frame() {
                Content = page
            };
            NotificationsStackPanel.Children.Add(inviteFrame);

            Action<Object, RoutedEventArgs> closeAction = (sender, e) => {
                NotificationsStackPanel.Children.Remove(inviteFrame);
            };

            page.AcceptButton.Click += new RoutedEventHandler(closeAction);
            page.RejectButton.Click += new RoutedEventHandler(closeAction);

            SetNotificationEntryCloseTimer(inviteFrame);
        }

        public void PushInviteNotification(PartyInvitationPage page) {
            if(InviteNotificationAlreadyShown(page.Player.Nickname)) {
                return;
            }
            AddInviteNotificationFrame(page);
        }
    }
}
