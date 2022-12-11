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
    /// Interaction logic for DeadScreenPage.xaml
    /// </summary>
    public partial class DeadScreenPage : Page {
        public DeadScreenPage() {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            App.Current.ResetApp();
            App.Current.CloseSecondaryPanel();
        }
    }
}
