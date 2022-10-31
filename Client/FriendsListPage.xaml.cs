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
    /// <summary>
    /// Interaction logic for FriendsListPage.xaml
    /// </summary>
    public partial class FriendsListPage : Page {
        public FriendsListPage() {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            App.Current.CloseFriendsList();

        }
    }
}
