using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Core.Game;

namespace Service {
    /// <summary>
    /// Data contract for player
    /// </summary>
    [DataContract]
    public class Player {
        public enum StatusType {
            Offline,
            Online,
            InGame
        }

        [IgnoreDataMember]
        public string SessionId { get; set; }

        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int Avatar { get; set; }

        [DataMember]
        public int GamesCount { get; set; }

        [DataMember]
        public int WinsCount { get; set; }

        [DataMember]
        public DateTime Registered { get; set; }

        [DataMember]
        public StatusType Status { get; set; }

        [DataMember]
        public bool IsGuest { get; set; }

        [IgnoreDataMember]
        public List<Player> Friends { get; set; }

        [IgnoreDataMember]
        public IPlayerManagerCallback PlayerManagerCallbackChannel { get; set; }

        [IgnoreDataMember]
        public Party CurrentParty { get; set; }

        [IgnoreDataMember]
        public IPartyManagerCallback PartyManagerCallbackChannel { get; set; }

        [IgnoreDataMember]
        public Tile?[] Rack { get; set; }

        [DataMember]
        public int Score { get; set; }

        [IgnoreDataMember]
        public int TurnPassesCount { get; set; }

        [IgnoreDataMember]
        public IPartyGameCallback PartyGameCallbackChannel { get; set; }

        [IgnoreDataMember]
        public IPartyChatCallback PartyChatCallbackChannel { get; set; }

        public int UsedRackTiles() {
            int count = 0;
            foreach(var tile in Rack) {
                if(tile == null) {
                    count++;
                }
            }
            return count;
        }

        public Tile? PopTileFromRack(int index) {
            if(index < 0 || index > Rack.Length) {
                return null;
            }
            var tile = Rack[index];
            Rack[index] = null;
            return tile;
        }

        public void Dispose() {
            if(CurrentParty != null) {
                CurrentParty.PlayerLeaves(this);
            }
        }

        public Player() {
            SessionId = Guid.NewGuid().ToString();
        }

        public Player(DataAccess.Player playerData) {
            Nickname = playerData.Nickname;
            Email = playerData.Email;
            Avatar = playerData.Avatar;
            GamesCount = playerData.Games;
            WinsCount = playerData.Wins;
            Registered = playerData.Registered;
            SessionId = Guid.NewGuid().ToString();
            IsGuest = false;
        }
    }
}
