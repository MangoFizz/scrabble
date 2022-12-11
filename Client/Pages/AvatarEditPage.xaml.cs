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
    /// Interaction logic for AvatarEditPage.xaml
    /// </summary>
    public partial class AvatarEditPage : Page {

        private Button GetAvatarItem(short avatarId) {
            var avatarItem = new Button();
            avatarItem.Background = Brushes.White;
            avatarItem.Margin = new Thickness(5);
            avatarItem.Padding = new Thickness(4);

            var avatarImage = new Image();
            avatarImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/images/avatars/avatar_{avatarId}.png"));
            avatarImage.Width = 165;
            avatarImage.Height = 165;
            avatarItem.Content = avatarImage;

            return avatarItem;
        }

        private void SetUpAvatarSelector() {
            for(short i = 0; i < 5; i++) {
                var avatarIndex = i;
                var avatarItem = GetAvatarItem(i);
                avatarItem.Click += (sender, e) => {
                    NavigationService.GoBack();
                    App.Current.PlayerManagerClient.UpdatePlayerAvatar(avatarIndex);
                };
                AvatarsPanel.Children.Add(avatarItem);
            }
        }

        public AvatarEditPage() {
            InitializeComponent();
            SetUpAvatarSelector();
        }

        private void RectagleMouseLeft_ButtonUp(object sender, MouseButtonEventArgs e) {
            App.Current.CloseSecondaryPanel();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
        }
    }
}
