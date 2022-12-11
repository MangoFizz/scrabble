using Client.GameService;
using Client.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client {
    /// <summary>
    /// Interaction logic for PartyGamePage.xaml
    /// </summary>
    public partial class PartyGamePage : Page {
        class BoardSlot {
            public Border Container { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }

        private PartyChatPage Chat { get; set; }
        private PartyGameClient Client { get; set; }
        private GameBoardSlot[,] Board { get; set; }
        private GameTile?[] Rack { get; set; }
        private Player CurrentTurn { get; set; }
        private Grid DraggedTileItem { get; set; }
        private Point DraggedTileStartingPosition { get; set; }
        private BoardSlot FocusedBoardSlot { get; set; }
        private bool CanPlaceTiles { get; set; }
        private bool TurnStarted { get; set; }

        public PartyGamePage(PartyChatPage chatPage) {
            InitializeComponent();

            Board = new GameBoardSlot[15, 15];
            Rack = new GameTile?[7];

            SetUpScoreTable();

            ChatFrame.Content = chatPage;
            Chat = chatPage;

            var context = new InstanceContext(this);
            Client = new PartyGameClient(context);
            Client.ConnectPartyGame(App.Current.SessionId);

            CanPlaceTiles = false;
            TurnStarted = false;
        }

        private void SetUpScoreTable() {
            var party = App.Current.CurrentParty;
            var players = party.Players;

            Player1ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[0].Nickname);
            Player1Score.Content = "0";

            if(players.Length >= 2) {
                Player2ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[1].Nickname);
                Player2Score.Content = "0";
            }
            else {
                Player2ScoreLabel.Visibility = Visibility.Hidden;
                Player2Score.Visibility = Visibility.Hidden;
            }

            if(players.Length >= 3) {
                Player3ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[2].Nickname);
                Player3Score.Content = "0";
            }
            else {
                Player3ScoreLabel.Visibility = Visibility.Hidden;
                Player3Score.Visibility = Visibility.Hidden;
            }

            if(players.Length >= 4) {
                Player4ScoreLabel.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_SCORE_LABEL_FORMAT, players[3].Nickname);
                Player4Score.Content = "0";
            }
            else {
                Player4ScoreLabel.Visibility = Visibility.Hidden;
                Player4Score.Visibility = Visibility.Hidden;
            }

            TilesLeftCount.Content = "--";

            PlayerTurnMessage.Content = Properties.Resources.PARTY_LOBBY_WAITING_FOR_PLAYERS;
        }

        private BoardSlot GetBoardDragOverSlot(Point dragObjectPosition) {
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

            var slot = BoardGrid.Children[slotRow * 15 + slotColumn] as Border;

            if(slot.Child != null) {
                return null;
            }

            var elem = new BoardSlot() {
                Column = slotColumn,
                Row = slotRow
            };
            elem.Container = slot;

            return elem;
        }

        private SolidColorBrush GetBoardSlotColor(GameBoardSlotBonus slotBonus) {
            SolidColorBrush color;
            switch(slotBonus) {
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
            return color;
        }

        private FontFamily GetTileFontFamily() {
            return new FontFamily(new Uri("pack://application:,,,/"), "./Resources/fonts/#Tiles Regular");
        }

        private Grid GetBoardSlotItemGlyph(char letter) {
            if(letter == ' ') {
                letter = '"';
            }
            var boardSlotItem = new Grid();
            var boardSlotGlyphBackground = new Label() {
                Content = "/",
                Foreground = Brushes.White,
                FontSize = 36,
                Padding = new Thickness(0, 1, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = GetTileFontFamily()
            };
            var boardSlotGlyphForeground = new Label() {
                Content = letter.ToString().ToLower(),
                Foreground = Brushes.Black,
                FontSize = 36,
                Padding = new Thickness(0, 1, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = GetTileFontFamily()
            };
            boardSlotItem.Children.Add(boardSlotGlyphBackground);
            boardSlotItem.Children.Add(boardSlotGlyphForeground);
            return boardSlotItem;
        }

        private Border GetBoardSlotItem(GameBoardSlot boardSlot) {
            var slotItemContainer = new Border() {
                Background = GetBoardSlotColor(boardSlot.Bonus),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(0),
                Opacity = 0.75
            };

            if(boardSlot.Tile.HasValue) {
                slotItemContainer.Child = GetBoardSlotItemGlyph((char)boardSlot.Tile.Value);
            }

            slotItemContainer.MouseEnter += (sender, e) => {
                slotItemContainer.BorderBrush = Brushes.Black;
                slotItemContainer.BorderThickness = new Thickness(slotItemContainer.Child == null ? 2 : 1);
                slotItemContainer.Opacity = 1;
            };

            slotItemContainer.MouseLeave += (sender, e) => {
                slotItemContainer.BorderBrush = Brushes.Gray;
                slotItemContainer.BorderThickness = new Thickness(1);
                slotItemContainer.Opacity = slotItemContainer.Child == null ? 0.75 : 1;
            };

            return slotItemContainer;
        }

        private void UpdateBoardGrid() {
            BoardGrid.Children.Clear();
            for(int x = 0; x < 15; x++) {
                for(int y = 0; y < 15; y++) {
                    var slot = Board[x, y];
                    var slotItemContainer = GetBoardSlotItem(slot);
                    Grid.SetColumn(slotItemContainer, x);
                    Grid.SetRow(slotItemContainer, y);
                    BoardGrid.Children.Add(slotItemContainer);
                }
            }
        }

        private void AddRackTileItemGlyph(Grid rackTileItemContainer, char letter) {
            var fontFamily = GetTileFontFamily();

            if(letter == ' ') {
                letter = '!';
            }

            var tileLetterBackground = new Label() {
                Content = ".",
                Width = 60,
                Height = 60,
                Foreground = Brushes.White,
                FontSize = 40,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = fontFamily
            };

            var tileLetterForeground = new Label() {
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
            
            rackTileItemContainer.Children.Add(tileLetterBackground);
            rackTileItemContainer.Children.Add(tileLetterForeground);
        }

        private Grid GetRackTileItem(char letter, int rackSlotIndex) {
            var tileItemContainer = new Grid();
            tileItemContainer.Opacity = 0.9;
            tileItemContainer.HorizontalAlignment = HorizontalAlignment.Left;
            tileItemContainer.VerticalAlignment = VerticalAlignment.Top;
            tileItemContainer.Margin = new Thickness(RackBorder.Margin.Left + (60 * RackCanvas.Children.Count), RackBorder.Margin.Top, 0, 0);

            AddRackTileItemGlyph(tileItemContainer, letter);

            tileItemContainer.MouseEnter += (sender, e) => {
                tileItemContainer.Opacity = 1;
            };

            tileItemContainer.MouseLeave += (sender, e) => {
                tileItemContainer.Opacity = 0.9;
            };

            tileItemContainer.PreviewMouseDown += (sender, e) => {
                if(!CanPlaceTiles) {
                    return;
                }
                DraggedTileItem = sender as Grid;
                DraggedTileStartingPosition = new Point() {
                    X = tileItemContainer.Margin.Left,
                    Y = tileItemContainer.Margin.Top
                };
            };

            tileItemContainer.PreviewMouseUp += (sender, e) => {
                if(DraggedTileItem != null) {
                    if(FocusedBoardSlot != null) {
                        Client.PlaceTile(rackSlotIndex, FocusedBoardSlot.Row, FocusedBoardSlot.Column);
                        PassTurnButton.Content = Properties.Resources.PARTY_GAME_END_TURN_BUTTON;

                        RackCanvas.Children.Remove(DraggedTileItem);

                        var draggedTileLabel = DraggedTileItem.Children[1] as Label;
                        var draggedTileLetter = draggedTileLabel.Content.ToString();
                        FocusedBoardSlot.Container.Child = GetBoardSlotItemGlyph(draggedTileLetter[0]);
                        FocusedBoardSlot = null;

                        TurnStarted = true;
                    }
                    else {
                        DraggedTileItem.Margin = new Thickness(DraggedTileStartingPosition.X, DraggedTileStartingPosition.Y, 0, 0);
                    }

                    DraggedTileItem = null;
                    RackCanvas.ReleaseMouseCapture();
                }
            };

            tileItemContainer.MouseLeave += (sender, e) => {
                if(DraggedTileItem != null && DraggedTileItem == sender) {
                    DraggedTileItem.Margin = new Thickness(DraggedTileStartingPosition.X, DraggedTileStartingPosition.Y, 0, 0);
                    DraggedTileItem = null;
                    RackCanvas.ReleaseMouseCapture();
                }
            };

            return tileItemContainer;
        }

        private void UpdateRackGrid() {
            RackCanvas.Children.Clear();
            for(int i = 0; i < 7; i++) {
                var tileIndex = i;
                var tile = Rack[i];

                if(tile.HasValue) {
                    char letter = '!';
                    if(tile != GameTile.Wildcard) {
                        letter = (char)tile.Value;
                    }
                    var tileContainer = GetRackTileItem(letter, tileIndex);
                    RackCanvas.Children.Add(tileContainer);
                }
                else {
                    RackCanvas.Children.Add(new Grid());

                }
            }
        }

        private void UpdateBoardSlotItemsFocus(Point currentCursorPosition) {
            var slot = GetBoardDragOverSlot(currentCursorPosition);
            if(slot != null) {
                var slotContainer = slot.Container;
                if(FocusedBoardSlot != null && FocusedBoardSlot.Container != slotContainer) {
                    FocusedBoardSlot.Container.BorderBrush = Brushes.Gray;
                    FocusedBoardSlot.Container.BorderThickness = new Thickness(1);
                }
                slotContainer.BorderThickness = new Thickness(slotContainer.Child != null ? 1 : 2);
                slotContainer.BorderBrush = Brushes.Black;
                FocusedBoardSlot = slot;
            }
            else if(FocusedBoardSlot != null) {
                var slotContainer = FocusedBoardSlot.Container;
                slotContainer.BorderBrush = Brushes.Gray;
                slotContainer.BorderThickness = new Thickness(1);
                FocusedBoardSlot = null;
            }
        }

        private void UpdateDraggedRackTilePosition(Point currentCursorPosition) {
            var positionX = currentCursorPosition.X - DraggedTileItem.ActualWidth / 2;
            var positionY = currentCursorPosition.Y - DraggedTileItem.ActualHeight / 2;
            DraggedTileItem.Margin = new Thickness(positionX, positionY, 0, 0);
        }

        private void RackCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs eventArgs) {
            if(DraggedTileItem == null) {
                return;
            }
            var currentPoint = eventArgs.GetPosition(sender as IInputElement);
            UpdateDraggedRackTilePosition(currentPoint);
            UpdateBoardSlotItemsFocus(currentPoint);
        }

        private void PassTurnButton_Click(object sender, RoutedEventArgs e) {
            if(CanPlaceTiles) {
                CanPlaceTiles = false;
                PassTurnButton.Content = Properties.Resources.PARTY_GAME_PASS_TURN_BUTTON;
                if(TurnStarted) {
                    Client.EndTurn();
                }
                else {
                    Client.PassTurn();
                }
            }
        }
    }

    public partial class PartyGamePage : IPartyGameCallback {
        public void SendInvalidTilePlacingError() {
            PlayerTurnMessage.Content = Properties.Resources.PARTY_GAME_INVALID_TILE_PLACING_ERROR;
            PlayerTurnMessage.Foreground = Brushes.Red;
            Task.Delay(2000).ContinueWith((t) => {
                Dispatcher.Invoke(() => {
                    UpdatePlayerTurnText();
                });
            });
        }

        public void UpdateBagTilesLeft(int amount) {
            TilesLeftCount.Content = amount.ToString();
        }

        public void UpdateBoard(GameBoardSlot[][] board) {
            for(int x = 0; x < 15; x++) {
                for(int y = 0; y < 15; y++) {
                    Board[x, y] = board[x][y];
                }
            }

            UpdateBoardGrid();
        }

        public void UpdatePlayerRack(GameTile?[] rack) {
            for(int i = 0; i < 7; i++) {
                Rack[i] = rack[i];
            }

            UpdateRackGrid();
        }

        public void UpdatePlayerScore(Player player, int score) {
            var players = App.Current.CurrentParty.Players;
            int playerIndex;
            for(playerIndex = 0; playerIndex < players.Length; playerIndex++) {
                if(players[playerIndex].Nickname == player.Nickname) {
                    break;
                }
            }

            Label playerScore;
            switch(playerIndex) {
                case 0:
                    playerScore = Player1Score;
                    break;
                case 1:
                    playerScore = Player2Score;
                    break;
                case 2:
                    playerScore = Player3Score;
                    break;
                case 3:
                    playerScore = Player4Score;
                    break;
                default:
                    throw new Exception("Invalid player index");
            }
            playerScore.Content = score.ToString();
        }

        private void UpdatePlayerTurnText() {
            PlayerTurnMessage.Foreground = Brushes.White;
            if(CurrentTurn.Nickname == App.Current.Player.Nickname) {
                PlayerTurnMessage.Content = Properties.Resources.PARTY_GAME_YOUR_TURN;
            }
            else {
                PlayerTurnMessage.Content = string.Format(Properties.Resources.PARTY_GAME_PLAYER_TURN_FORMAT, CurrentTurn.Nickname);
            }
        }

        public void UpdatePlayerTurn(Player player) {
            CurrentTurn = player;
            if(player.Nickname == App.Current.Player.Nickname) {
                CanPlaceTiles = true;
                TurnStarted = false;
            }
            UpdatePlayerTurnText();
        }

        public void GameEnd(Party party) {
            App.Current.CurrentParty = party;
            App.Current.SecondaryFrame.Content = new PartyGameResultsPage();
        }
    }
}
