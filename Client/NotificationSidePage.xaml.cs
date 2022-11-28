using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Interaction logic for NotificationSidePage.xaml
    /// </summary>
    public partial class NotificationSidePage : Page {
        public NotificationSidePage() {
            InitializeComponent();
        }

        public void PushInviteNotification(PartyInvitationPage page) {
            foreach(var child in NotificationsStackPanel.Children) {
                if(typeof(Frame).IsInstanceOfType(child)) {
                    var childFrame = (Frame)child;
                    if(typeof(PartyInvitationPage).IsInstanceOfType(childFrame.Content)) {
                        var invitationPage = (PartyInvitationPage)childFrame.Content;
                        if(invitationPage.Player.Nickname == page.Player.Nickname) {
                            return;
                        }
                    }
                }
            }

            var frame = new Frame();
            page.VerticalAlignment = VerticalAlignment.Top;
            frame.Content = page;
            NotificationsStackPanel.Children.Add(frame);

            Action<Object, RoutedEventArgs> closeAction = (sender, e) => {
                NotificationsStackPanel.Children.Remove(frame);
            };

            page.AcceptButton.Click += new RoutedEventHandler(closeAction);
            page.RejectButton.Click += new RoutedEventHandler(closeAction);

            Task.Factory.StartNew(() => {
                Thread.Sleep(5600);
                Dispatcher.Invoke(() => {
                    NotificationsStackPanel.Children.Remove(frame);
                });
            });
        }
    }
}
