using Core;
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

        public bool GameEndByTurnPasses() {
            foreach(var player in Players) {
                if(player.TurnPassesCount <= MAX_TURN_PASSES) {
                    return false;
                }
            }
            return true;
        }

        public void PlayerLeaves(Player player) {
            Players.Remove(player);
            foreach(var p in Players) {
                p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
            }

            if(Leader == player) {
                Leader = Players[0];
                foreach(var p in Players) {
                    p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(Leader);
                }
            }
        }

        public Party() {
            Players = new List<Player>();
            Id = Guid.NewGuid().ToString();
        }
    }
}
