using Client.GameService;
using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Client {
    /// <summary>
    /// Interaction logic for PartyGamePage.xaml
    /// </summary>
    public partial class PartyGamePage : Page, IPartyGameCallback {
        private PartyGameClient Client { get; set; }
        private GameBoardSlot[,] Board { get; set; }
        private GameTile[] Rack { get; set; }
        private Grid DragTile { get; set; }
        private Point DragTileStartingPosition { get; set; }
        private Rectangle FocusedBoardSlot { get; set; }

        public PartyGamePage(PartyChatPage chatPage) {
            InitializeComponent();

            // Initialize resources
            Board = new GameBoardSlot[15, 15];
            Rack = new GameTile[7];

            // Restore chat
            ChatFrame.Content = chatPage;

            // Create client
            var context = new InstanceContext(this);
            Client = new PartyGameClient(context);
            Client.ConnectPartyGame(App.Current.SessionId);
        }

        private Rectangle GetBoardDragOverRectangle(Point dragObjectPosition) {
            var board = BoardGrid;
            var startPoint = new Point() {
                X = board.Margin.Left,
                Y = board.Margin.Top
            };
            var slotSize = board.ActualWidth / 15;
            var slotRow = (int)((dragObjectPosition.X - startPoint.X) / slotSize);
            var slotColumn = (int)((dragObjectPosition.Y - startPoint.Y) / slotSize);

            if(slotRow < 0 || slotRow > 14 || slotColumn < 0 || slotColumn > 14) {
                return null;
            }

            // Get the rectangle from Grid
            var slot = BoardGrid.Children[slotRow * 15 + slotColumn] as Rectangle;

            return slot;
        }

        private void UpdateBoardGrid() {
            // Update board grid
            BoardGrid.Children.Clear();

            for(int x = 0; x < 15; x++) {
                for(int y = 0; y < 15; y++) {
                    var slot = Board[x, y];
                    SolidColorBrush color;
                    switch(slot.Bonus) {
                        case GameBoardSlotBonus.DoubleLetter:
                            color = Brushes.SkyBlue;
                            break;

                        case GameBoardSlotBonus.DoubleWord:
                            color = Brushes.Orange;
                            break;

                        case GameBoardSlotBonus.TripleLetter:
                            color = Brushes.RoyalBlue;
                            break;

                        case GameBoardSlotBonus.TripleWord:
                            color = Brushes.OrangeRed;
                            break;

                        case GameBoardSlotBonus.Center:
                            color = Brushes.Yellow;
                            break;

                        case GameBoardSlotBonus.None:
                        default:
                            color = Brushes.White;
                            break;
                    }

                    var rectangle = new Rectangle() {
                        Fill = color,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    };

                    rectangle.Opacity = 0.75;

                    rectangle.MouseEnter += (sender, e) => {
                        rectangle.Stroke = Brushes.Black;
                        rectangle.StrokeThickness = 2;
                        rectangle.Opacity = 1;
                    };

                    rectangle.MouseLeave += (sender, e) => {
                        rectangle.Stroke = Brushes.Gray;
                        rectangle.StrokeThickness = 1;
                        rectangle.Opacity = 0.75;
                    };

                    rectangle.AllowDrop = true;

                    Grid.SetColumn(rectangle, x);
                    Grid.SetRow(rectangle, y);
                    BoardGrid.Children.Add(rectangle);
                }
            }
        }

        private void UpdateRackGrid() {
            for(int i = 0; i < 7; i++) {
                var tile = Rack[i];
                var letter = (char)tile;
                var fontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/fonts/#Tiles Regular");

                // Account for wildcard
                if(letter == ' ') {
                    letter = '!';
                }

                var tileToken = new Label() {
                    Content = ".",
                    Width = 60,
                    Height = 60,
                    Foreground = Brushes.White,
                    FontSize = 40,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                    FontFamily = fontFamily
                };

                var tileBackground = new Label() {
                    Content = letter.ToString(),
                    Width = 60,
                    Height = 60,
                    Opacity = 0.9,
                    Foreground = Brushes.Black,
                    FontSize = 40,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                    FontFamily = fontFamily
                };

                var rackPosition = new Point() {
                    X = RackBorder.Margin.Left,
                    Y = RackBorder.Margin.Top
                };

                var tileContainer = new Grid();
                tileContainer.Opacity = 0.9;
                tileContainer.HorizontalAlignment = HorizontalAlignment.Left;
                tileContainer.VerticalAlignment = VerticalAlignment.Top;
                tileContainer.Margin = new Thickness(rackPosition.X + (60 * RackCanvas.Children.Count), rackPosition.Y, 0, 0);
                tileContainer.Children.Add(tileToken);
                tileContainer.Children.Add(tileBackground);

                tileContainer.MouseEnter += (sender, e) => {
                    tileContainer.Opacity = 1;
                };

                tileContainer.MouseLeave += (sender, e) => {
                    tileContainer.Opacity = 0.9;
                };

                tileContainer.PreviewMouseDown += (sender, e) => {
                    DragTile = sender as Grid;
                    DragTileStartingPosition = new Point() {
                        X = tileContainer.Margin.Left,
                        Y = tileContainer.Margin.Top
                    };
                };
                
                tileContainer.PreviewMouseUp += (sender, e) => {
                    if(DragTile != null && DragTile == sender) {
                        DragTile.Margin = new Thickness(DragTileStartingPosition.X, DragTileStartingPosition.Y, 0, 0);
                        DragTile = null;
                        RackCanvas.ReleaseMouseCapture();
                    }
                };

                tileContainer.MouseLeave += (sender, e) => {
                    if(DragTile != null) {
                        DragTile.Margin = new Thickness(DragTileStartingPosition.X, DragTileStartingPosition.Y, 0, 0);
                        DragTile = null;
                        RackCanvas.ReleaseMouseCapture();
                    }
                };

                RackCanvas.Children.Add(tileContainer);
            }
        }
        
        private void RackCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if(DragTile == null) {
                return;
            }
            var currentPoint = e.GetPosition(sender as IInputElement);
            DragTile.Margin = new Thickness(currentPoint.X - DragTile.ActualWidth / 2, currentPoint.Y - DragTile.ActualHeight / 2, 0, 0);
            var slot = GetBoardDragOverRectangle(currentPoint);
            if(slot != null) {
                if(FocusedBoardSlot != null && FocusedBoardSlot != slot) {
                    FocusedBoardSlot.Stroke = Brushes.Gray;
                    FocusedBoardSlot.StrokeThickness = 1;
                }
                slot.StrokeThickness = 2;
                slot.Stroke = Brushes.Black;
                FocusedBoardSlot = slot;
            }
            else if(FocusedBoardSlot != null) {
                FocusedBoardSlot.Stroke = Brushes.Gray;
                FocusedBoardSlot.StrokeThickness = 1;
                FocusedBoardSlot = null;
            }
        }

        public void SendInvalidTilePlacingError() {
            throw new NotImplementedException();
        }

        public void UpdateBagTilesLeft(int amount) {
            
        }

        public void UpdateBoard(GameBoardSlot[][] board) {
            // Convert again from a jagged array to a 2D array
            for(int x = 0; x < 15; x++) {
                for(int y = 0; y < 15; y++) {
                    Board[x, y] = board[x][y];
                }
            }

            UpdateBoardGrid();
        }

        public void UpdatePlayerRack(GameTile[] rack) {
            Rack = rack;

            UpdateRackGrid();
        }

        public void UpdatePlayerScore(int score) {
            throw new NotImplementedException();
        }

        public void UpdatePlayerTurn(Player player) {
            throw new NotImplementedException();
        }
    }
}
