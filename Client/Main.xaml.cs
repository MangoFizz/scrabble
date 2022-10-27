using System.Windows;
using System.Windows.Controls;

namespace Client {
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class Main : Page {
        public Main() {
            InitializeComponent();
        }

        private void exitButtonClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            this.NavigationService.Navigate(new Chat());
        }
    }
}
