﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service {
    [DataContract]
    public partial class Party {
        private const int MAX_TURN_PASSES = 2;
        
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string InviteCode { get; set; }

        [DataMember]
        public Player Leader { get; set; }

        [DataMember]
        public List<Player> Players { get; set; }

        [IgnoreDataMember]
        public Game Game { get; set; }

        [IgnoreDataMember]
        public int CurrentPlayerTurn { get; set; }

        [IgnoreDataMember]
        public int TimeLimitMins { get; set; }

        [IgnoreDataMember]
        public Timer Timer { get; set; }

        public Player NextTurn() {
            CurrentPlayerTurn = (CurrentPlayerTurn + 1) % Players.Count;
            return Players[CurrentPlayerTurn];
        }

        public bool IsFull() {
            return Players.Count == Game.MAX_PLAYERS;
        }

        public void SetGameEndTimer(int timeLimit) {
            TimeLimitMins = timeLimit;
            Task.Delay(TimeLimitMins * 60 * 1000).ContinueWith(t => {
                if(Game != null) {
                    EndGame();
                }
            });
            Log.Info($"Game end timer set for party {InviteCode} to {TimeLimitMins} minutes.");
        }

        public void JoinPlayer(Player player) {
            Players.Add(player);
            player.CurrentParty = this;
            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceivePartyPlayerJoin(player);
            }
            Log.Info($"Player {player.Nickname} joined party {Id}.");
        }

        public void SetRandomTurn() {
            var randomNumber = Core.Random.Next(0, Players.Count);
            CurrentPlayerTurn = randomNumber;
            foreach(var p in Players) {
                p.PartyGameCallbackChannel.UpdatePlayerTurn(Players[randomNumber]);
            }
        }

        public void TransferOwnership(Player player) {
            Leader = player;
            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(player);
            }
            Log.Info($"Party {Id} leader transfered to {player.Nickname}.");
        }

        public void EndGame() {
            if(Game != null) {
                SaveResult();
                foreach(var p in Players) {
                    p.PartyGameCallbackChannel.GameEnd(this);
                }
                foreach(var p in Players) {
                    p.PartyGameCallbackChannel = null;
                    p.Rack = null;
                    p.Score = 0;
                }
                Game = null;
            }
        }

        public bool GameEndByTurnPasses() {
            foreach(var player in Players) {
                if(player.TurnPassesCount <= MAX_TURN_PASSES) {
                    return false;
                }
            }
            return true;
        }

        public bool GamePlayersAreReady() {
            foreach(var p in Players) {
                if(p.PartyGameCallbackChannel == null) {
                    return false;
                }
            }
            return true;
        }

        public void UpdatePlayersGame() {
            foreach(var player in Players) {
                player.SendGameUpdate();
            }
        }

        public void RefreshPlayersTilesLeftCount() {
            foreach(var p in Players) {
                if(p.PartyGameCallbackChannel != null) {
                    p.PartyGameCallbackChannel.UpdateBagTilesLeft(Game.Bag.Count);
                }
            }
        }

        public void CreateGame(Game.SupportedLanguage language, int timeLimit) {
            Game = new Game(language);
            TimeLimitMins = timeLimit;
            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceiveGameStart();
            }
            SetGameEndTimer(timeLimit);
        }

        public void PlayerLeaves(Player player) {
            bool isPlayerTurn = Game != null && Players[CurrentPlayerTurn] == player;

            if(Game != null) {
                if(isPlayerTurn) {
                    CurrentPlayerTurn = CurrentPlayerTurn % (Players.Count - 1);
                }
                else if(Players.IndexOf(player) < CurrentPlayerTurn) {
                    CurrentPlayerTurn--;
                }
            }

            Players.Remove(player);

            if(Game != null) {
                if(Players.Count == 1) {
                    EndGame();
                    return;
                }
            }

            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
                if(isPlayerTurn) {
                    p.PartyGameCallbackChannel.UpdatePlayerTurn(Players[CurrentPlayerTurn]);
                }
            }

            if(Players.Count > 0 && Leader == player) {
                Leader = Players[0];
                foreach(var p in Players) {
                    p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(Leader);
                }
            }
        }

        private void SaveResult() {
            var winner = Players.Max(p => p.Score);
            var winnerPlayer = Players.First(p => p.Score == winner);
            var gameId = PlayerManager.SaveGame(winnerPlayer.Nickname);
            var placements = Players.OrderByDescending(p => p.Score).ToList();
            foreach(var player in Players) {
                PlayerManager.SaveGameResult(player.Nickname, gameId, player.Score, placements.IndexOf(player) + 1);
            }
        }

        public Party() {
            Players = new List<Player>();
            Id = Guid.NewGuid().ToString();
        }
    }
}
