using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Service {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class GameService { 
        public void Shutdown() {
            DisconnectAllPlayerClients();
        }
    }

    public partial class GameService : IPlayerManager {
        private List<Player> Players = new List<Player>();

        private void FetchPlayerFriendsStatus(Player player) {
            for(var i = 0; i < player.Friends.Count; i++) {
                var friend = player.Friends[i];
                var onlinePlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                if(onlinePlayer != null) {
                    if(onlinePlayer.CurrentParty != null) {
                        friend.Status = Player.StatusType.InGame;
                    }
                    else {
                        friend.Status = Player.StatusType.Online;
                    }
                }
            }
        }

        private void NotifyFriendsPlayerState(Player player) {
            if(!player.IsGuest) {
                foreach(var friend in player.Friends) {
                    var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                    if(connectedPlayer != null) {
                        connectedPlayer.PlayerManagerCallbackChannel.UpdateFriendStatus(player, player.Status);
                    }
                }
            }
        }

        private void SendFriendRequestNotification(string nickname, Player sender) {
            var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == nickname);
            if(connectedPlayer != null) {
                connectedPlayer.PlayerManagerCallbackChannel.ReceiveFriendRequest(sender);
            }
        }

        private void DisconnectPlayerClient(string nickname) {
            var player = Players.FirstOrDefault(p => p.Nickname == nickname);
            if(player != null) {
                player.PlayerManagerCallbackChannel.Disconnect(DisconnectionReason.DuplicatePlayerSession);
            }
            Players.Remove(player);
        }

        private void DisconnectAllPlayerClients() {
            foreach(var player in Players) {
                player.PlayerManagerCallbackChannel.Disconnect(DisconnectionReason.ServerShutdown);
            }
            Players.Clear();
        }

        public void Login(string nickname, string password) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var authenticationResult = PlayerManager.AuthenticatePlayer(nickname, password);
            if(authenticationResult == PlayerManager.PlayerAuthResult.Success) {
                DisconnectPlayerClient(nickname);
                var playerData = PlayerManager.GetPlayerData(nickname);
                var player = new Player(playerData) {
                    PlayerManagerCallbackChannel = currentCallbackChannel,
                    Status = Player.StatusType.Online
                };
                Players.Add(player);
                currentCallbackChannel.LoginResponseHandler(authenticationResult, player, player.SessionId);
                player.FetchFriendList();
                NotifyFriendsPlayerState(player);
            }
            else {
                currentCallbackChannel.LoginResponseHandler(authenticationResult, null, null);
            }
        }

        public void LoginAsGuest() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = new Player() {
                Nickname = $"Guest #{Players.Count + 900}",
                Avatar = 3,
                Status = Player.StatusType.Online,
                IsGuest = true,
                PlayerManagerCallbackChannel = currentCallbackChannel
            };
            Players.Add(player);
            currentCallbackChannel.LoginResponseHandler(PlayerManager.PlayerAuthResult.Success, player, player.SessionId);
        }

        public void Logout() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(data => data.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(player != null) {
                player.Status = Player.StatusType.Offline;
                NotifyFriendsPlayerState(player);
                player.Dispose();
                Players.Remove(player);
            }
            else {
                Log.Warning("Unknown player called to log out without being logged in.");
            }
        }

        public void RegisterPlayer(string nickname, string password, string email) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var registerResult = PlayerManager.RegisterPlayer(nickname, password, email);
            currentCallbackChannel.RegisterPlayerResponseHandler(registerResult);
        }

        public void SendFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var sender = Players.Find(data => data.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(sender != null) {
                if(!sender.IsGuest) {
                    var requestResult = PlayerManager.RequestFriendship(sender.Nickname, nickname);
                    if(requestResult == PlayerManager.PlayerFriendRequestResult.Success) {
                        SendFriendRequestNotification(nickname, sender);
                    }
                    currentCallbackChannel.SendFriendRequestResponseHandler(requestResult);
                }
                else {
                    Log.Warning("Guest player tried to send a friend request.");
                }
            }
            else {
                Log.Warning("Unknown player tried to send a friend request without being logged in.");
            }
        }

        public void AcceptFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(!currentPlayer.IsGuest) {
                    var requestResponseResult = PlayerManager.AnswerFriendshipRequest(currentPlayer.Nickname, nickname, true);
                    if(requestResponseResult == PlayerManager.PlayerFriendshipRequestAnswer.Success) {
                        var senderPlayer = Players.Find(p => p.Nickname == nickname);
                        if(senderPlayer != null) {
                            senderPlayer.PlayerManagerCallbackChannel.ReceiveFriendAdd(currentPlayer);
                            currentPlayer.PlayerManagerCallbackChannel.ReceiveFriendAdd(senderPlayer);
                            senderPlayer.PlayerManagerCallbackChannel.UpdateFriendStatus(currentPlayer, Player.StatusType.Online);
                            currentPlayer.PlayerManagerCallbackChannel.UpdateFriendStatus(senderPlayer, Player.StatusType.Online);
                        }
                        else {
                            var senderPlayerData = PlayerManager.GetPlayerData(nickname);
                            senderPlayer = new Player(senderPlayerData);
                            currentPlayer.PlayerManagerCallbackChannel.ReceiveFriendAdd(senderPlayer);
                        }
                        currentPlayer.Friends.Add(senderPlayer);
                    }
                }
                else {
                    Log.Warning("Guest player tried to accept a friend request (??).");
                }
            }
            else {
                Log.Warning("Unknown player tried to accept a friend request without being logged in.");
            }
        }

        public void DeclineFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(player != null) {
                if(!player.IsGuest) {
                    PlayerManager.AnswerFriendshipRequest(player.Nickname, nickname, false);
                }
                else {
                    Log.Warning("Guest player tried to decline a friend request.");
                }
            }
            else {
                Log.Warning("Unknown player tried to decline a friend request without being logged in.");
            }
        }

        public void GetFriendList() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(player != null) {
                if(!player.IsGuest) {
                    player.FetchFriendList();
                    FetchPlayerFriendsStatus(player);
                    currentCallbackChannel.GetFriendListResponseHandler(player.Friends.ToArray());
                }
                else {
                    Log.Warning("Guest player tried to get friend list.");
                }
            }
            else {
                Log.Warning("Unknown player tried to get friend list without being logged in.");
            }
        }

        public void GetFriendRequests() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(!currentPlayer.IsGuest) {
                    var playerFriendRequestsData = PlayerManager.GetPrendingFriendRequest(currentPlayer.Nickname);
                    var friendRequests = playerFriendRequestsData.Select(data => new Player(data));
                    currentCallbackChannel.GetFriendRequestsResponseHandler(friendRequests.ToArray());
                }
                else {
                    Log.Warning("Guest player tried to get friend requests.");
                }
            }
            else {
                Log.Warning("Unknown player tried to get friend requests without being logged in.");
            }
        }

        public void VerifyPlayer(string nickname, string password, string code) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var playerVerificationResult = PlayerManager.VerifyPlayer(nickname, password, code);
            currentCallbackChannel.VerificationResponseHandler(playerVerificationResult);
        }

        public void ResendVerificationCode(string nickname, string password) {
            var authenticationResult = PlayerManager.AuthenticatePlayer(nickname, password);
            if(authenticationResult == PlayerManager.PlayerAuthResult.Success) {
                PlayerManager.ResendVerificationCode(nickname);
            }
            else {
                Log.Warning($"Player {nickname} tried to resend verification code but authentication failed.");
            }
        }

        public void UpdatePlayerAvatar(short avatarId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(!currentPlayer.IsGuest) {
                    PlayerManager.UpdatePlayerAvatar(currentPlayer.Nickname, avatarId);
                    currentPlayer.Avatar = avatarId;
                    currentCallbackChannel.UpdatePlayerAvatarCallback(avatarId);
                }
                else {
                    Log.Warning("Guest player tried to update its avatar.");
                }
            }
            else {
                Log.Warning("Unknown player tried to update avatar without being logged in.");
            }
        }
    }

    public partial class GameService : IPartyChat {
        public void ConnectPartyChat(string sessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var currentPlayer = Players.Find(p => p.SessionId == sessionId);
            if(currentPlayer != null) {
                currentPlayer.PartyChatCallbackChannel = currentCallbackChannel;
            }
            else {
                Log.Warning("Unknown player tried to connect to party chat without being logged in.");
            }
        }

        public void Say(string message) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var player = Players.FirstOrDefault(u => u.PartyChatCallbackChannel == currentCallbackChannel);
            if(player != null) {
                foreach(var p in Players) {
                    p.PartyChatCallbackChannel.Receive(player, message);
                }
            }
            else {
                Log.Warning("Unknown player tried to say something in party chat without being logged in.");
            }
        }
    }

    public partial class GameService : IPartyManager {
        private List<Party> Parties = new List<Party>();

        private string GenerateInviteCode() {
            var code = Core.Random.Next(0x1000, 0x10000).ToString("x2");
            if(Parties.Any(p => p.InviteCode == code)) {
                return GenerateInviteCode();
            }
            else {
                return code.ToUpper();
            }
        }

        public void ConnectPartyManager(string sessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var player = Players.Find(p => p.SessionId == sessionId);
            if(player != null) {
                player.PartyManagerCallbackChannel = currentCallbackChannel;
            }
            else {
                Log.Warning("Unknown player tried to connect to party manager without being logged in.");
            }
        }

        public Party CreateParty() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = new Party() {
                    Leader = currentPlayer,
                    Players = new List<Player>() { currentPlayer },
                    InviteCode = GenerateInviteCode()
                };
                Parties.Add(party);
                currentPlayer.CurrentParty = party;
                return party;
            }
            else {
                Log.Warning("Unknown player tried to create a party without being logged in.");
                return null;
            }
        }

        public void AcceptInvitation(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var inviteSender = Players.Find(p => p.Nickname == player.Nickname);
                if(inviteSender != null) {
                    var party = inviteSender.CurrentParty;
                    if(party != null) {
                        currentPlayer.PartyManagerCallbackChannel = currentCallbackChannel;
                        currentPlayer.PartyManagerCallbackChannel.AcceptInvitationCallback(party);
                        party.JoinPlayer(currentPlayer);
                    }
                }
            }
            else {
                Log.Warning("Unknown player tried to accept an invitation without being logged in.");
            }
        }

        public void InviteFriend(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            var targetPlayer = Players.FirstOrDefault(u => u.Nickname == player.Nickname);
            if(targetPlayer != null && currentPlayer != null) {
                if(currentPlayer.CurrentParty != null && targetPlayer.CurrentParty == null) {
                    targetPlayer.PartyManagerCallbackChannel.ReceiveInvitation(currentPlayer, currentPlayer.CurrentParty.Id);
                }
            }
            else {
                Log.Warning("Unknown player tried to invite a friend without being logged in.");
            }
        }

        public void KickPlayer(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {
                    if(currentPlayer.Nickname == party.Leader.Nickname) {
                        var targetPlayer = party.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
                        if(targetPlayer != null) {
                            party.Players.Remove(targetPlayer);
                            targetPlayer.CurrentParty = null;
                            targetPlayer.PartyManagerCallbackChannel.ReceivePartyKick(targetPlayer);
                            foreach(var p in party.Players) {
                                p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(targetPlayer);
                            }
                        }
                        else {
                            Log.Warning($"Player {currentPlayer.Nickname} tried to kick a player that is not in the party.");
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to kick a player without being the party leader.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to kick a player without being in a party.");
                }
            }
            else {
                Log.Warning("Unknown player tried to kick a player without being logged in.");
            }
        }

        public void LeaveParty() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var player = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(player != null) {
                var party = player.CurrentParty;
                if(party != null) {
                    player.CurrentParty = null;
                    party.PlayerLeaves(player);
                    player.Status = Player.StatusType.Online;
                    NotifyFriendsPlayerState(player);
                    if(party.Players.Count > 0) {
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
                        }
                        if(player.Nickname == party.Leader.Nickname) {
                            party.TransferOwnership(party.Players[0]);
                        }
                    }
                    else {
                        Parties.Remove(party);
                        Log.Info($"Empty party {party.Id} has been removed.");
                    }
                }
                else {
                    Log.Warning($"Player {player.Nickname} tried to leave a party without being in one.");
                }
            }
            else {
                Log.Warning("Unknown player tried to leave a party without being logged in.");
            }
        }

        public void StartGame(Game.SupportedLanguage language, int timeLimitMins) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {

                    if(party.Leader.Nickname == currentPlayer.Nickname) {
                        if(party.Players.Count >= Game.MIN_PLAYERS) {
                            foreach(var p in party.Players) {
                                p.PartyManagerCallbackChannel.StartGameCallback(GameStartResult.Success);
                            }
                            party.CreateGame(language, timeLimitMins);
                        }
                        else {
                            currentPlayer.PartyManagerCallbackChannel.StartGameCallback(GameStartResult.NotEnoughPlayers);
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to start a game without being the party leader.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to start a game without being in a party.");
                }
            }
            else {
                Log.Warning("Unknown player tried to start a game without being logged in.");
            }
        }
        
        public void TransferLeadership(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {
                    if(currentPlayer.Nickname == party.Leader.Nickname) {
                        var targetPlayer = party.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
                        if(targetPlayer != null) {
                            party.Leader = targetPlayer;
                            foreach(var p in party.Players) {
                                p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(targetPlayer);
                            }
                        }
                        else {
                            Log.Warning($"Player {currentPlayer.Nickname} tried to transfer leadership to a player that is not in the party.");
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to transfer leadership without being the party leader.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to transfer leadership without being in a party.");
                }
            }
            else {
                Log.Warning("Unknown player tried to transfer leadership without being logged in.");
            }
        }

        public void UpdateTimeLimitSetting(int time) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {
                    if(currentPlayer.Nickname == party.Leader.Nickname) {
                        foreach(var p in party.Players) {
                            if(p == currentPlayer) {
                                continue;
                            }
                            p.PartyManagerCallbackChannel.ReceivePartyTimeLimitUpdate(time);
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to update the time limit setting without being the party leader.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to update the time limit setting without being in a party.");
                }
            }
            else {
                Log.Warning("Unknown player tried to update the time limit setting without being logged in.");
            }
        }

        public void UpdateLanguageSetting(Game.SupportedLanguage language) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {
                    if(currentPlayer.Nickname == party.Leader.Nickname) {
                        foreach(var p in party.Players) {
                            if(p == currentPlayer) {
                                continue;
                            }
                            p.PartyManagerCallbackChannel.ReceivePartyLanguageUpdate(language);
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to update the language setting without being the party leader.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to update the language setting without being in a party.");
                }
            }
            else {
                Log.Warning("Unknown player tried to update the language setting without being logged in.");
            }
        }

        public void JoinParty(string inviteCode) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = Parties.Find(p => p.InviteCode == inviteCode.ToUpper());
                if(party != null) {
                    if(!party.IsFull()) {
                        currentPlayer.PartyManagerCallbackChannel = currentCallbackChannel;
                        currentCallbackChannel.JoinPartyCallback(JoinPartyResult.Success, party);
                        party.JoinPlayer(currentPlayer);
                    }
                    else {
                        currentCallbackChannel.JoinPartyCallback(JoinPartyResult.PartyIsFull, null);
                    }
                }
                else {
                    currentCallbackChannel.JoinPartyCallback(JoinPartyResult.PartyNotFound, null);
                }
            }
            else {
                Log.Warning("Unknown player tried to join a party without being logged in.");
            }
        }
    }

    public partial class GameService : IPartyGame {
        public void ConnectPartyGame(string playerSessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var player = Players.Find(p => p.SessionId == playerSessionId);
            if(player != null) {
                if(player.CurrentParty != null) {
                    player.PartyGameCallbackChannel = currentCallbackChannel;
                    var party = player.CurrentParty;
                    player.CreateRack();
                    player.SendGameUpdate();
                    party.RefreshPlayersTilesLeftCount();
                    if(party.GamePlayersAreReady()) {
                        party.SetRandomTurn();
                    }
                    player.TurnPassesCount = 0;
                }
                else {
                    Log.Warning($"Player {player.Nickname} tried to connect to a party game without being in a party.");
                }
            }
            else {
                Log.Warning($"Unknown player tried to connect to a party game without being logged in.");
            }
        }

        public void EndTurn() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(currentPlayer.CurrentParty != null) {
                    currentPlayer.RefillRack();
                    var party = currentPlayer.CurrentParty;
                    var nextPlayerTurn = party.NextTurn();
                    currentPlayer.TurnPassesCount = 0;
                    foreach(var p in party.Players) {
                        p.PartyGameCallbackChannel.UpdateBagTilesLeft(party.Game.Bag.Count);
                        p.PartyGameCallbackChannel.UpdatePlayerTurn(nextPlayerTurn);
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to end his turn without being in a party.");
                }
            }
            else {
                Log.Warning($"Unknown player tried to end his turn without being logged in.");
            }
        }

        public void PassTurn() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(currentPlayer.CurrentParty != null) {
                    var party = currentPlayer.CurrentParty;
                    currentPlayer.TurnPassesCount++;
                    if(party.GameEndByTurnPasses()) {
                        party.EndGame();
                        return;
                    }
                    var nextPlayerTurn = party.NextTurn();
                    foreach(var p in party.Players) {
                        p.PartyGameCallbackChannel.UpdatePlayerTurn(nextPlayerTurn);
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to pass his turn without being in a party.");
                }
            }
            else {
                Log.Warning($"Unknown player tried to pass his turn without being logged in.");
            }
        }

        public void PlaceTile(int rackTileIndex, int x, int y) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                if(currentPlayer.CurrentParty != null) {
                    var party = currentPlayer.CurrentParty;
                    if(party.Players[party.CurrentPlayerTurn] == currentPlayer) {
                        var tile = currentPlayer.PopTileFromRack(rackTileIndex);
                        if(tile.HasValue) {
                            try {
                                var points = party.Game.PlaceTile(tile.Value, x, y);
                                if(currentPlayer.UsedRackTiles() == currentPlayer.Rack.Length) {
                                    points += 50;
                                }
                                currentPlayer.Score += points;
                            }
                            catch(InvalidOperationException) {
                                currentPlayer.Rack[rackTileIndex] = tile;
                                currentCallbackChannel.SendInvalidTilePlacingError();
                                Log.Warning($"Player {currentPlayer.Nickname} tried to place a tile at an invalid position.");
                            }
                            party.UpdatePlayersGame();
                        }
                        else {
                            Log.Warning($"Player {currentPlayer.Nickname} tried to place a tile that doesn't exist in his rack.");
                        }
                    }
                    else {
                        Log.Warning($"Player {currentPlayer.Nickname} tried to place a tile while it's not his turn.");
                    }
                }
                else {
                    Log.Warning($"Player {currentPlayer.Nickname} tried to place a tile without being in a party.");
                }
            }
            else {
                Log.Warning($"Unknown player tried to place a tile without being logged in.");
            }
        }
    }
}
