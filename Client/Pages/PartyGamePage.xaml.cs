using Client.GameService;
using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client {
    /// <summary>
    /// Interaction logic for PartyGamePage.xaml
    /// </summary>
    public partial class PartyGamePage : Page, IPartyGameCallback {
        private PartyGameClient Client { get; set; }
        private GameBoardSlot[,] Board { get; set; }
        private GameTile?[] Rack { get; set; }
        private Grid DraggedTile { get; set; }
        private Point DraggedTileStartingPosition { get; set; }
        private Border FocusedBoardSlot { get; set; }

        public PartyGamePage(PartyChatPage chatPage) {
            InitializeComponent();

            // Initialize resources
            Board = new GameBoardSlot[15, 15];
            Rack = new GameTile?[7];

            // Set up score table
            SetUpScoreTable();

            // Restore chat
            ChatFrame.Content = chatPage;

            // Create client
            var context = new InstanceContext(this);
            Client = new PartyGameClient(context);
            Client.ConnectPartyGame(App.Current.SessionId);
        }

        private void SetUpScoreTable() {
            var party = App.Current.CurrentParty;
            var players = party.Players;

            Player1ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[0].Nickname);
            Player1Score.Content = "0";

            if(players.Length == 2) {
                Player2ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[1].Nickname);
                Player2Score.Content = "0";
            }
            else {
                Player2ScoreLabel.Visibility = Visibility.Hidden;
                Player2Score.Visibility = Visibility.Hidden;
            }

            if(players.Length == 3) {
                Player3ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[2].Nickname);
                Player3Score.Content = "0";
            }
            else {
                Player3ScoreLabel.Visibility = Visibility.Hidden;
                Player3Score.Visibility = Visibility.Hidden;
            }

            if(players.Length == 4) {
                Player4ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[3].Nickname);
                Player4Score.Content = "0";
            }
            else {
                Player4ScoreLabel.Visibility = Visibility.Hidden;
                Player4Score.Visibility = Visibility.Hidden;
            }

            TilesLeftCount.Content = "--";

            PlayerTurnMessage.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_TURN_FORMAT, players[0].Nickname);
        }

        private Border GetBoardDragOverSlot(Point dragObjectPosition) {
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
            var slot = BoardGrid.Children[slotRow * 15 + slotColumn] as Border;

            // If the slot is NOT empty, do not allow the tile to be dropped!
            if(slot.Child != null) {
                return null;
            }

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

                    var slotContainer = new Border() {
                        Background = color,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(0),
                        Opacity = 0.75
                    };

                    slotContainer.MouseEnter += (sender, e) => {
                        slotContainer.BorderBrush = Brushes.Black;
                        slotContainer.BorderThickness = new Thickness(slotContainer.Child == null ? 2 : 1);
                        slotContainer.Opacity = 1;
                    };

                    slotContainer.MouseLeave += (sender, e) => {
                        slotContainer.BorderBrush = Brushes.Gray;
                        slotContainer.BorderThickness = new Thickness(1);
                        slotContainer.Opacity = slotContainer.Child == null ? 0.75 : 1;
                    };

                    slotContainer.AllowDrop = true;

                    Grid.SetColumn(slotContainer, x);
                    Grid.SetRow(slotContainer, y);
                    BoardGrid.Children.Add(slotContainer);
                }
            }
        }

        private void UpdateRackGrid() {
            for(int i = 0; i < 7; i++) {
                var tileIndex = i;
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
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontFamily = fontFamily
                };

                var tileBackground = new Label() {
                    Content = letter.ToString(),
                    Width = 60,
                    Height = 60,
                    Opacity = 0.9,
                    Foreground = Brushes.Black,
                    FontSize = 40,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
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
                    DraggedTile = sender as Grid;
                    DraggedTileStartingPosition = new Point() {
                        X = tileContainer.Margin.Left,
                        Y = tileContainer.Margin.Top
                    };
                };
                
                tileContainer.PreviewMouseUp += (sender, e) => {
                    if(DraggedTile != null) {
                        if(FocusedBoardSlot != null) {
                            var draggedTileLabel = DraggedTile.Children[1] as Label;
                            var draggedTileLetter = draggedTileLabel.Content.ToString();

                            var slot = FocusedBoardSlot;

                            // Place tile into the focused slot
                            slot.Child = new Label() {
                                Content = draggedTileLetter.ToLower(),
                                Foreground = Brushes.Black,
                                FontSize = 36,
                                Padding = new Thickness(0,1,0,0),
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                FontFamily = fontFamily
                            };

                            RackCanvas.Children.Remove(DraggedTile);
                            Rack[tileIndex] = null;

                            FocusedBoardSlot = null;
                        }
                        else {
                            // If there is no focused slot, just return the tile to the rack
                            DraggedTile.Margin = new Thickness(DraggedTileStartingPosition.X, DraggedTileStartingPosition.Y, 0, 0);
                        }

                        DraggedTile = null;
                        RackCanvas.ReleaseMouseCapture();
                    }
                };

                tileContainer.MouseLeave += (sender, e) => {
                    if(DraggedTile != null && DraggedTile == sender) {
                        DraggedTile.Margin = new Thickness(DraggedTileStartingPosition.X, DraggedTileStartingPosition.Y, 0, 0);
                        DraggedTile = null;
                        RackCanvas.ReleaseMouseCapture();
                    }
                };

                RackCanvas.Children.Add(tileContainer);
            }
        }
        
        private void RackCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if(DraggedTile == null) {
                return;
            }

            // Update the position of the dragged object
            var currentPoint = e.GetPosition(sender as IInputElement);
            DraggedTile.Margin = new Thickness(currentPoint.X - DraggedTile.ActualWidth / 2, currentPoint.Y - DraggedTile.ActualHeight / 2, 0, 0);
            
            // Handle board slot mouse focus
            var slot = GetBoardDragOverSlot(currentPoint);
            if(slot != null) {
                if(FocusedBoardSlot != null && FocusedBoardSlot != slot) {
                    FocusedBoardSlot.BorderBrush = Brushes.Gray;
                    FocusedBoardSlot.BorderThickness = new Thickness(1);
                }
                slot.BorderThickness = new Thickness(slot.Child != null ? 1 : 2);
                slot.BorderBrush = Brushes.Black;
                FocusedBoardSlot = slot;
            }
            else if(FocusedBoardSlot != null) {
                FocusedBoardSlot.BorderBrush = Brushes.Gray;
                FocusedBoardSlot.BorderThickness = new Thickness(1);
                FocusedBoardSlot = null;
            }
        }

        public void SendInvalidTilePlacingError() {
            throw new NotImplementedException();
        }

        public void UpdateBagTilesLeft(int amount) {
            TilesLeftCount.Content = amount.ToString();
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
            for(int i = 0; i < 7; i++) {
                Rack[i] = rack[i];
            }

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
