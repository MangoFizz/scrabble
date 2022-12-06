using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core {
    public class Game {
        public const int MAX_PLAYERS = 4;
        public const int MIN_PLAYERS = 2;

        public enum SupportedLanguage {
            en_US,
            es_MX
        };

        public enum Tile {
            A = 'A',
            B, C, D, E, F, G, H, I, J, K, L, M,
            N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
            Ñ = 'Ñ',
            Wildcard = ' ',
        };

        public enum BoardSlotBonus {
            None,
            Center,
            TripleWord,
            DoubleWord,
            TripleLetter,
            DoubleLetter
        };

        public class BoardSlot {
            public Tile? Tile { get; set; }
            public BoardSlotBonus Bonus { get; set; }

            public BoardSlot(BoardSlotBonus bonus) {
                Tile = null;
                Bonus = bonus;
            }
        };

        public SupportedLanguage Language { get; set; }

        public BoardSlot[,] Board { get; set; }

        public List<Tile> Bag { get; set; }

        public Game(SupportedLanguage language) {
            Board = new BoardSlot[15, 15];
            Bag = new List<Tile>();
            Language = language;

            // Initialize the board
            for(int i = 0; i < 15; i++) {
                for(int j = 0; j < 15; j++) {
                    Board[i, j] = new BoardSlot(BoardSlotBonus.None);
                }
            }

            SetBoardBonusSlots();
            FillBag();
            ShuffleBag();
        }

        private void SetBoardBonusSlots() {
            Action<int, int, BoardSlotBonus> setBonus = (x, y, bonus) => {
                Board[x, y].Bonus = bonus;
            };

            // Set the center bonus
            setBonus(7, 7, BoardSlotBonus.Center);

            // Set the triple word bonuses
            setBonus(0, 0, BoardSlotBonus.TripleWord);
            setBonus(0, 7, BoardSlotBonus.TripleWord);
            setBonus(0, 14, BoardSlotBonus.TripleWord);
            setBonus(7, 0, BoardSlotBonus.TripleWord);
            setBonus(7, 14, BoardSlotBonus.TripleWord);
            setBonus(14, 0, BoardSlotBonus.TripleWord);
            setBonus(14, 7, BoardSlotBonus.TripleWord);
            setBonus(14, 14, BoardSlotBonus.TripleWord);

            // Set the double word bonuses
            setBonus(1, 1, BoardSlotBonus.DoubleWord);
            setBonus(1, 13, BoardSlotBonus.DoubleWord);
            setBonus(2, 2, BoardSlotBonus.DoubleWord);
            setBonus(2, 12, BoardSlotBonus.DoubleWord);
            setBonus(3, 3, BoardSlotBonus.DoubleWord);
            setBonus(3, 11, BoardSlotBonus.DoubleWord);
            setBonus(4, 4, BoardSlotBonus.DoubleWord);
            setBonus(4, 10, BoardSlotBonus.DoubleWord);
            setBonus(7, 7, BoardSlotBonus.DoubleWord);
            setBonus(10, 4, BoardSlotBonus.DoubleWord);
            setBonus(10, 10, BoardSlotBonus.DoubleWord);
            setBonus(11, 3, BoardSlotBonus.DoubleWord);
            setBonus(11, 11, BoardSlotBonus.DoubleWord);
            setBonus(12, 2, BoardSlotBonus.DoubleWord);
            setBonus(12, 12, BoardSlotBonus.DoubleWord);
            setBonus(13, 1, BoardSlotBonus.DoubleWord);
            setBonus(13, 13, BoardSlotBonus.DoubleWord);

            // Set the triple letter bonuses
            setBonus(1, 5, BoardSlotBonus.TripleLetter);
            setBonus(1, 9, BoardSlotBonus.TripleLetter);
            setBonus(5, 1, BoardSlotBonus.TripleLetter);
            setBonus(5, 5, BoardSlotBonus.TripleLetter);
            setBonus(5, 9, BoardSlotBonus.TripleLetter);
            setBonus(5, 13, BoardSlotBonus.TripleLetter);
            setBonus(9, 1, BoardSlotBonus.TripleLetter);
            setBonus(9, 5, BoardSlotBonus.TripleLetter);
            setBonus(9, 9, BoardSlotBonus.TripleLetter);
            setBonus(9, 13, BoardSlotBonus.TripleLetter);
            setBonus(13, 5, BoardSlotBonus.TripleLetter);
            setBonus(13, 9, BoardSlotBonus.TripleLetter);

            // Set the double letter bonuses
            setBonus(0, 3, BoardSlotBonus.DoubleLetter);
            setBonus(0, 11, BoardSlotBonus.DoubleLetter);
            setBonus(2, 6, BoardSlotBonus.DoubleLetter);
            setBonus(2, 8, BoardSlotBonus.DoubleLetter);
            setBonus(3, 0, BoardSlotBonus.DoubleLetter);
            setBonus(3, 7, BoardSlotBonus.DoubleLetter);
            setBonus(3, 14, BoardSlotBonus.DoubleLetter);
            setBonus(6, 2, BoardSlotBonus.DoubleLetter);
            setBonus(6, 6, BoardSlotBonus.DoubleLetter);
            setBonus(6, 8, BoardSlotBonus.DoubleLetter);
            setBonus(6, 12, BoardSlotBonus.DoubleLetter);
            setBonus(7, 3, BoardSlotBonus.DoubleLetter);
            setBonus(7, 11, BoardSlotBonus.DoubleLetter);
            setBonus(8, 2, BoardSlotBonus.DoubleLetter);
            setBonus(8, 6, BoardSlotBonus.DoubleLetter);
            setBonus(8, 8, BoardSlotBonus.DoubleLetter);
            setBonus(8, 12, BoardSlotBonus.DoubleLetter);
            setBonus(11, 0, BoardSlotBonus.DoubleLetter);
            setBonus(11, 7, BoardSlotBonus.DoubleLetter);
            setBonus(11, 14, BoardSlotBonus.DoubleLetter);
            setBonus(12, 6, BoardSlotBonus.DoubleLetter);
            setBonus(12, 8, BoardSlotBonus.DoubleLetter);
            setBonus(14, 3, BoardSlotBonus.DoubleLetter);
            setBonus(14, 11, BoardSlotBonus.DoubleLetter);
        }

        private void FillBag() {
            Action<Tile, int> addTile = (tile, count) => {
                for(int i = 0; i < count; i++) {
                    Bag.Add(tile);
                }
            };

            switch(Language) {
                case SupportedLanguage.en_US:
                    addTile(Tile.A, 9);
                    addTile(Tile.B, 2);
                    addTile(Tile.C, 2);
                    addTile(Tile.D, 4);
                    addTile(Tile.E, 12);
                    addTile(Tile.F, 2);
                    addTile(Tile.G, 3);
                    addTile(Tile.H, 2);
                    addTile(Tile.I, 9);
                    addTile(Tile.J, 1);
                    addTile(Tile.K, 1);
                    addTile(Tile.L, 4);
                    addTile(Tile.M, 2);
                    addTile(Tile.N, 6);
                    addTile(Tile.O, 8);
                    addTile(Tile.P, 2);
                    addTile(Tile.Q, 1);
                    addTile(Tile.R, 6);
                    addTile(Tile.S, 4);
                    addTile(Tile.T, 6);
                    addTile(Tile.U, 4);
                    addTile(Tile.V, 2);
                    addTile(Tile.W, 2);
                    addTile(Tile.X, 1);
                    addTile(Tile.Y, 2);
                    addTile(Tile.Z, 1);
                    addTile(Tile.Wildcard, 2);
                    break;

                case SupportedLanguage.es_MX:
                    addTile(Tile.A, 12);
                    addTile(Tile.B, 2);
                    addTile(Tile.C, 5);
                    addTile(Tile.D, 5);
                    addTile(Tile.E, 12);
                    addTile(Tile.F, 1);
                    addTile(Tile.G, 2);
                    addTile(Tile.H, 3);
                    addTile(Tile.I, 6);
                    addTile(Tile.J, 1);
                    addTile(Tile.L, 6);
                    addTile(Tile.M, 2);
                    addTile(Tile.N, 5);
                    addTile(Tile.Ñ, 1);
                    addTile(Tile.O, 9);
                    addTile(Tile.P, 2);
                    addTile(Tile.Q, 1);
                    addTile(Tile.R, 7);
                    addTile(Tile.S, 6);
                    addTile(Tile.T, 4);
                    addTile(Tile.U, 5);
                    addTile(Tile.V, 1);
                    addTile(Tile.W, 1);
                    addTile(Tile.X, 1);
                    addTile(Tile.Y, 1);
                    addTile(Tile.Z, 1);
                    addTile(Tile.Wildcard, 2);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void ShuffleBag() {
            Random random = new Random();
            for(int i = 0; i < Bag.Count; i++) {
                int randomIndex = random.Next(0, Bag.Count);
                Tile tempTile = Bag[i];
                Bag[i] = Bag[randomIndex];
                Bag[randomIndex] = tempTile;
            }
        }
    }
}
