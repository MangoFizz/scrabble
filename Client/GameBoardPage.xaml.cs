using System;
using System.Collections.Generic;
using System.Drawing;
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
using Color = System.Drawing.Color;

namespace Client {
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class GameBoardPage : Page {
        public GameBoardPage() {
            InitializeComponent();
            GenerateBoard();
            GenerateArtilesPlayer1();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            App.Current.MainWindow.MainFrame.GoBack();
        }


        private void GenerateBoard() {
            for(int i = 0; i < 81; i++) {
                var ButtonCell = new Button();
                ButtonCell.Width = 54;
                ButtonCell.Height = 54;
                ButtonCell.Background = new SolidColorBrush(Colors.LightGreen);
                ButtonCell.BorderBrush = new SolidColorBrush(Colors.Transparent);
                BordGame.Children.Add(ButtonCell);
            }
        }

        private void GenerateArtilesPlayer1() {
            for(int i = 0; i < 7; i++) {
                var ButtonLetter = new Button();
                ButtonLetter.Width = 54;
                ButtonLetter.Height = 54;
                ButtonLetter.BorderBrush = new SolidColorBrush(Colors.Transparent);
                ArtilPlayer1.Children.Add(ButtonLetter);
            }
        }
    }

    
}
