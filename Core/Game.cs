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

            public BoardSlot() { }
        };

        public static Dictionary<SupportedLanguage, List<string>> WordsDictionariesCache { get; set; }

        public SupportedLanguage Language { get; set; }

        public List<string> WordsDictionary;

        public BoardSlot[,] Board { get; set; }

        public List<Tile> Bag { get; set; }
        private bool BoardIsEmpty { get; set; }

        public Game(SupportedLanguage language) {
            Board = new BoardSlot[15, 15];
            Bag = new List<Tile>();
            Language = language;

            for(int i = 0; i < 15; i++) {
                for(int j = 0; j < 15; j++) {
                    Board[i, j] = new BoardSlot(BoardSlotBonus.None);
                }
            }

            SetBoardBonusSlots();
            FillBag();
            ShuffleBag();
            LoadWordDictionary();
            BoardIsEmpty = true;
        }

        private static string GetLanguageName(SupportedLanguage language) {
            switch(language) {
                case SupportedLanguage.es_MX:
                    return "es";
                case SupportedLanguage.en_US:
                default:
                    return "en";
            }
        }

        private void LoadWordDictionary() {
            if(WordsDictionariesCache == null) {
                WordsDictionariesCache = new Dictionary<SupportedLanguage, List<string>>();
            }

            if(!WordsDictionariesCache.ContainsKey(Language)) {
                WordsDictionariesCache[Language] = new List<string>();
                var dictionaryResourceName = string.Format(Properties.Resources.WORDS_DICTIONARY_NAME_FORMAT, GetLanguageName(Language));
                var dictionaryResource = Properties.Resources.ResourceManager.GetString(dictionaryResourceName);
                var lines = dictionaryResource.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines) {
                    string word = "";
                    bool wordIsValid = true;
                    foreach(char c in line.ToUpper()) {
                        if(!char.IsLetter(c)) {
                            wordIsValid = false;
                            break;
                        }

                        if(!Enum.IsDefined(typeof(Tile), (int)c)) {
                            wordIsValid = false;
                            break;
                        }

                        word += c;
                    }
                    if(word.Length == 1) {
                        wordIsValid = false;
                    }
                    if(wordIsValid) {
                        var normalizedWord = word.Normalize(NormalizationForm.FormD);
                        WordsDictionariesCache[Language].Add(normalizedWord);
                    }
                }
                WordsDictionariesCache[Language] = lines.ToList();
                Log.Info(string.Format("Loaded {0} words for language {1}", WordsDictionariesCache[Language].Count, Language));
            }


            WordsDictionary = WordsDictionariesCache[Language];
        }

        private void SetBoardBonusSlots() {
            Action<int, int, BoardSlotBonus> setBonus = (x, y, bonus) => {
                Board[x, y].Bonus = bonus;
            };

            setBonus(7, 7, BoardSlotBonus.Center);

            setBonus(0, 0, BoardSlotBonus.TripleWord);
            setBonus(0, 7, BoardSlotBonus.TripleWord);
            setBonus(0, 14, BoardSlotBonus.TripleWord);
            setBonus(7, 0, BoardSlotBonus.TripleWord);
            setBonus(7, 14, BoardSlotBonus.TripleWord);
            setBonus(14, 0, BoardSlotBonus.TripleWord);
            setBonus(14, 7, BoardSlotBonus.TripleWord);
            setBonus(14, 14, BoardSlotBonus.TripleWord);

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
                    //addTile(Tile.Wildcard, 2);
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
                    addTile(Tile.X, 1);
                    addTile(Tile.Y, 1);
                    addTile(Tile.Z, 1);
                    //addTile(Tile.Wildcard, 2);
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

        private int GetTileScore(Tile tile) {
            switch(Language) {
                case SupportedLanguage.en_US:
                    switch(tile) {
                        case Tile.A:
                        case Tile.E:
                        case Tile.I:
                        case Tile.O:
                        case Tile.U:
                        case Tile.L:
                        case Tile.N:
                        case Tile.R:
                        case Tile.S:
                        case Tile.T:
                            return 1;

                        case Tile.D:
                        case Tile.G:
                            return 2;

                        case Tile.B:
                        case Tile.C:
                        case Tile.M:
                        case Tile.P:
                            return 3;

                        case Tile.F:
                        case Tile.H:
                        case Tile.V:
                        case Tile.W:
                        case Tile.Y:
                            return 4;

                        case Tile.K:
                            return 5;

                        case Tile.J:
                        case Tile.X:
                            return 8;

                        case Tile.Q:
                        case Tile.Z:
                            return 10;

                        case Tile.Wildcard:
                            return 0;

                        default:
                            throw new NotImplementedException();
                    }

                case SupportedLanguage.es_MX:
                    switch(tile) {
                        case Tile.A:
                        case Tile.E:
                        case Tile.I:
                        case Tile.O:
                        case Tile.U:
                        case Tile.N:
                        case Tile.L:
                        case Tile.S:
                        case Tile.R:
                        case Tile.T:
                            return 1;

                        case Tile.D:
                        case Tile.G:
                            return 2;

                        case Tile.B:
                        case Tile.C:
                        case Tile.M:
                        case Tile.P:
                            return 3;

                        case Tile.F:
                        case Tile.H:
                        case Tile.V:
                        case Tile.Y:
                            return 4;

                        case Tile.Q:
                            return 5;

                        case Tile.Ñ:
                        case Tile.J:
                        case Tile.X:
                            return 8;

                        case Tile.Z:
                            return 10;

                        case Tile.Wildcard:
                            return 0;

                        default:
                            throw new NotImplementedException();
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public Tile[] TakeFromBag(int amount = 7) {
            Tile[] tiles = new Tile[amount];
            for(int i = 0; i < amount; i++) {
                tiles[i] = Bag[0];
                Bag.RemoveAt(0);
            }
            return tiles;
        }

        public string[] GetSlotTileChains(int x, int y) {
            string leftChain = "";
            for(int i = x - 1; i >= 0; i--) {
                if(Board[i, y].Tile == null || Board[i, y].Tile == Tile.Wildcard) {
                    break;
                }
                leftChain = ((char)Board[i, y].Tile).ToString() + leftChain;
            }

            string rightChain = "";
            for(int i = x + 1; i < Board.GetLength(0); i++) {
                if(Board[i, y].Tile == null || Board[i, y].Tile == Tile.Wildcard) {
                    break;
                }
                rightChain += ((char)Board[i, y].Tile).ToString();
            }

            string topChain = "";
            for(int i = y - 1; i >= 0; i--) {
                if(Board[x, i].Tile == null || Board[x, i].Tile == Tile.Wildcard) {
                    break;
                }
                topChain = ((char)Board[x, i].Tile).ToString() + topChain;
            }

            string bottomChain = "";
            for(int i = y + 1; i < Board.GetLength(1); i++) {
                if(Board[x, i].Tile == null || Board[x, i].Tile == Tile.Wildcard) {
                    break;
                }
                bottomChain += ((char)Board[x, i].Tile).ToString();
            }

            return new string[] { leftChain, rightChain, topChain, bottomChain };
        }

        private bool ValidatePosition(Tile tile, int x, int y) {
            if(BoardIsEmpty && x == 7 && y == 7) {
                return true;
            }

            var letter = ((char)tile).ToString();
            var chains = GetSlotTileChains(x, y);
            string leftWord = chains[0];
            string rightWord = chains[1];
            string topWord = chains[2];
            string bottomWord = chains[3];

            if(leftWord.Length == 0 && rightWord.Length == 0 && topWord.Length == 0 && bottomWord.Length == 0) {
                return false;
            }

            bool horizontalIsValid = true;
            if(leftWord.Length > 0 && rightWord.Length == 0) {
                horizontalIsValid = WordsDictionary.Any(word => word.StartsWith(leftWord + letter));
            }
            else if(leftWord.Length == 0 && rightWord.Length > 0) {
                horizontalIsValid = WordsDictionary.Any(word => word.EndsWith(letter + rightWord));
            }
            else if(leftWord.Length > 0 && rightWord.Length > 0) {
                horizontalIsValid = WordsDictionary.Any(word => word.Contains(leftWord + letter + rightWord));
            }

            bool verticalIsValid = true;
            if(topWord.Length > 0 && bottomWord.Length == 0) {
                verticalIsValid = WordsDictionary.Any(word => word.StartsWith(topWord + letter));
            }
            else if(topWord.Length == 0 && bottomWord.Length > 0) {
                verticalIsValid = WordsDictionary.Any(word => word.EndsWith(letter + bottomWord));
            }
            else if(topWord.Length > 0 && bottomWord.Length > 0) {
                verticalIsValid = WordsDictionary.Any(word => word.Contains(topWord + letter + bottomWord));
            }

            return horizontalIsValid && verticalIsValid;
        }

        private int CalculateTilePlacementScore(Tile tile, int x, int y) {
            var slot = Board[x, y];
            var tileScore = GetTileScore(tile);
            int score = 0;
            switch(slot.Bonus) {
                case BoardSlotBonus.DoubleLetter:
                    score = tileScore * 2;
                    break;

                case BoardSlotBonus.TripleLetter:
                    score = tileScore * 3;
                    break;

                case BoardSlotBonus.DoubleWord: {
                        var chains = GetSlotTileChains(x, y);
                        foreach(var chain in chains) {
                            if(chain.Length > 0) {
                                score += chain.Sum(c => GetTileScore((Tile)c));
                            }
                        }
                        score += tileScore * 2;
                        break;
                    }

                case BoardSlotBonus.TripleWord: {
                        var chains = GetSlotTileChains(x, y);
                        foreach(var chain in chains) {
                            if(chain.Length > 0) {
                                score += chain.Sum(c => GetTileScore((Tile)c)) * 2;
                            }
                        }
                        score += tileScore * 3;
                        break;
                    }

                default:
                    score = tileScore;
                    break;
            }
            return score;
        }

        public int PlaceTile(Tile tile, int x, int y) {
            if(x < 0 || x > 14 || y < 0 || y > 14) {
                throw new ArgumentOutOfRangeException();
            }

            if(Board[x, y].Tile != null || !ValidatePosition(tile, x, y)) {
                throw new InvalidOperationException();
            }

            Board[x, y].Tile = tile;
            BoardIsEmpty = false;
            var score = CalculateTilePlacementScore(tile, x, y);
            return score;
        }

        public BoardSlot[][] GetBoardJaggedArray() {
            BoardSlot[][] jaggedBoard = new BoardSlot[15][];
            for(int i = 0; i < 15; i++) {
                jaggedBoard[i] = new BoardSlot[15];
                for(int j = 0; j < 15; j++) {
                    jaggedBoard[i][j] = Board[i, j];
                }
            }
            return jaggedBoard;
        }
    }
}
