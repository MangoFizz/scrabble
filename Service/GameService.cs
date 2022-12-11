using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Service {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class GameService { 
        public void Dispose() {
            DisconnectAllPlayerClients();
        }
    }

        public partial class GameService : IPlayerManager {
        private List<Player> Players = new List<Player>();

        private void FetchPlayerFriendList(Player player) {
            var playerFriendsData = PlayerManager.GetPlayerFriendsData(player.Nickname);
            player.Friends = playerFriendsData.Select(data => new Player(data)).ToList();
        }

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
            if(player.IsGuest) {
                return;
            }
            foreach(var friend in player.Friends) {
                var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                if(connectedPlayer != null) {
                    connectedPlayer.PlayerManagerCallbackChannel.UpdateFriendStatus(player, player.Status);
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
                FetchPlayerFriendList(player);
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
            player.Status = Player.StatusType.Offline;
            NotifyFriendsPlayerState(player);
            Players.Remove(player);
        }

        public void RegisterPlayer(string nickname, string password, string email) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var registerResult = PlayerManager.RegisterPlayer(nickname, password, email);
            currentCallbackChannel.RegisterPlayerResponseHandler(registerResult);
        }

        public void SendFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var sender = Players.Find(data => data.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(sender.IsGuest) {
                return;
            }
            var requestResult = PlayerManager.RequestFriendship(sender.Nickname, nickname);
            if(requestResult == PlayerManager.PlayerFriendRequestResult.Success) {
                SendFriendRequestNotification(nickname, sender);
            }
            currentCallbackChannel.SendFriendRequestResponseHandler(requestResult);
        }

        public void AcceptFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer.IsGuest) {
                return;
            }
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

        public void DeclineFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(player.IsGuest) {
                return;
            }
            PlayerManager.AnswerFriendshipRequest(player.Nickname, nickname, false);
        }

        public void GetFriendList() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(player.IsGuest) {
                return;
            }
            FetchPlayerFriendList(player);
            FetchPlayerFriendsStatus(player);
            currentCallbackChannel.GetFriendListResponseHandler(player.Friends.ToArray());
        }

        public void GetFriendRequests() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer.IsGuest) {
                return;
            }
            var playerFriendRequestsData = PlayerManager.GetPrendingFriendRequest(currentPlayer.Nickname);
            var friendRequests = playerFriendRequestsData.Select(data => new Player(data));
            currentCallbackChannel.GetFriendRequestsResponseHandler(friendRequests.ToArray());
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
        }
    }

    public partial class GameService : IPartyChat {
        public void ConnectPartyChat(string sessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var currentPlayer = Players.Find(p => p.SessionId == sessionId);
            currentPlayer.PartyChatCallbackChannel = currentCallbackChannel;
        }

        public void Say(string message) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var player = Players.FirstOrDefault(u => u.PartyChatCallbackChannel == currentCallbackChannel);
            if(player != null) {
                foreach(var p in Players) {
                    p.PartyChatCallbackChannel.Receive(player, message);
                }
            }
        }

        public void Whisper(Player receiver, string message) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var player = Players.FirstOrDefault(u => u.PartyChatCallbackChannel == currentCallbackChannel);
            if(player != null) {
                receiver.PartyChatCallbackChannel.ReceiveWhisper(player, message);
            }
        }
    }

    public partial class GameService : IPartyManager {
        private List<Party> Parties = new List<Party>();

        private void TransferPartyOwnership(Player player, Party party) {
            if(player.Nickname == party.Leader.Nickname) {
                if(party.Players.Count() > 0) {
                    party.Leader = party.Players[0];
                    foreach(var p in party.Players) {
                        p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(party.Leader);
                    }
                }
            }
        }
        
        private void CreateGame(Party party, Game.SupportedLanguage language, int timeLimit) {
            var game = new Game(language);
            party.Game = game;
            party.TimeLimitMins = timeLimit;
            foreach(var p in party.Players) {
                p.PartyManagerCallbackChannel.ReceiveGameStart();
            }

            party.Timer = new System.Threading.Timer((o) => { }, null, 0, timeLimit * 60 * 1000);
        }

        private void JoinPlayerToParty(Player player, Party party) {
            party.Players.Add(player);
            player.CurrentParty = party;
            foreach(var p in party.Players) {
                p.PartyManagerCallbackChannel.ReceivePartyPlayerJoin(player);
            }
        }

        private string GenerateInviteCode() {
            var random = new Random();
            var code = random.Next(0x1000, 0xFFFF).ToString("x2");
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
        }

        public void CreateParty() {
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
                currentCallbackChannel.CreatePartyCallback(currentPlayer.CurrentParty);
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
                        player.PartyManagerCallbackChannel.AcceptInvitationCallback(party);
                        JoinPlayerToParty(player, party);
                    }
                }
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
        }

        public void KickPlayer(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null && currentPlayer.Nickname == party.Leader.Nickname) {
                    var targetPlayer = party.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
                    if(targetPlayer != null) {
                        party.Players.Remove(targetPlayer);
                        targetPlayer.CurrentParty = null;
                        targetPlayer.PartyManagerCallbackChannel.ReceivePartyKick(targetPlayer);
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(targetPlayer);
                        }
                    }
                }
            }
        }

        public void LeaveParty() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var player = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(player != null) {
                var party = player.CurrentParty;
                if(party != null) {
                    player.CurrentParty = null;
                    party.Players.Remove(player);
                    player.Status = Player.StatusType.Online;
                    NotifyFriendsPlayerState(player);
                    if(party.Players.Count > 0) {
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
                        }
                        TransferPartyOwnership(player, party);
                    }
                    else {
                        Parties.Remove(party);
                    }
                }
            }
        }

        public void StartGame(Game.SupportedLanguage language, int timeLimitMins) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null) {
                    if(party.Leader.Nickname == currentPlayer.Nickname) {
                        if(party.Players.Count < Game.MIN_PLAYERS) {
                            currentPlayer.PartyManagerCallbackChannel.StartGameCallback(GameStartResult.NotEnoughPlayers);
                            return;
                        }
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.StartGameCallback(GameStartResult.Success);
                        }
                        CreateGame(party, language, timeLimitMins);
                    }
                }
            }
        }
        
        public void TransferLeadership(Player player) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null && currentPlayer.Nickname == party.Leader.Nickname) {
                    var targetPlayer = party.Players.FirstOrDefault(p => p.Nickname == player.Nickname);
                    if(targetPlayer != null) {
                        party.Leader = targetPlayer;
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(targetPlayer);
                        }
                    }
                }
            }
        }

        public void UpdateTimeLimitSetting(int time) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null && currentPlayer.Nickname == party.Leader.Nickname) {
                    foreach(var p in party.Players) {
                        if(p == currentPlayer) {
                            continue;
                        }
                        p.PartyManagerCallbackChannel.ReceivePartyTimeLimitUpdate(time);
                    }
                }
            }
        }

        public void UpdateLanguageSetting(Game.SupportedLanguage language) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = currentPlayer.CurrentParty;
                if(party != null && currentPlayer.Nickname == party.Leader.Nickname) {
                    foreach(var p in party.Players) {
                        if(p == currentPlayer) {
                            continue;
                        }
                        p.PartyManagerCallbackChannel.ReceivePartyLanguageUpdate(language);
                    }
                }
            }
        }

        public void JoinParty(string inviteCode) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var currentPlayer = Players.Find(p => p.PartyManagerCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null) {
                var party = Parties.Find(p => p.InviteCode == inviteCode.ToUpper());
                if(party == null) {
                    currentCallbackChannel.JoinPartyCallback(JoinPartyResult.PartyNotFound, null);
                    return;
                }
                if(party.IsFull()) {
                    currentCallbackChannel.JoinPartyCallback(JoinPartyResult.PartyIsFull, null);
                    return;
                }
                currentPlayer.PartyManagerCallbackChannel = currentCallbackChannel;
                currentCallbackChannel.JoinPartyCallback(JoinPartyResult.Success, party);
                JoinPlayerToParty(currentPlayer, party);
            }
            currentCallbackChannel.JoinPartyCallback(JoinPartyResult.WhoAreYou, null);
        }
    }

    public partial class GameService : IPartyGame {
        private bool GamePlayersAreReady(Party party) {
            foreach(var p in party.Players) {
                if(p.PartyGameCallbackChannel == null) {
                    return false;
                }
            }
            return true;
        }

        private void CreatePlayerRack(Player player) {
            if(player.Rack != null) {
                return;
            }
            var party = player.CurrentParty;
            var game = party.Game;
            player.Rack = new Game.Tile?[7];
            var newRack = game.TakeFromBag();
            for(int i = 0; i < newRack.Length; i++) {
                player.Rack[i] = newRack[i];
            }
        }

        private void UpdatePlayerGame(Player player) {
            var callbackChannel = player.PartyGameCallbackChannel;
            if(callbackChannel == null) {
                return;
            }
            callbackChannel.UpdateBoard(player.CurrentParty.Game.GetBoardJaggedArray());
            callbackChannel.UpdatePlayerRack(player.Rack);
            callbackChannel.UpdatePlayerScore(player, player.Score);
        }

        private void UpdatePlayersGame(Party party) {
            foreach(var player in party.Players) {
                UpdatePlayerGame(player);
            }
        }

        private void RefreshGameTilesLeftCount(Party party) {
            foreach(var p in party.Players) {
                if(p.PartyGameCallbackChannel != null) {
                    p.PartyGameCallbackChannel.UpdateBagTilesLeft(party.Game.Bag.Count);
                }
            }
        }

        private void SetRandomTurn(Party party) {
            var random = new Random();
            var randomNumber = random.Next(0, party.Players.Count - 1);
            party.CurrentPlayerTurn = randomNumber;
            foreach(var p in party.Players) {
                p.PartyGameCallbackChannel.UpdatePlayerTurn(party.Players[randomNumber]);
            }
        }

        private void RefillPlayerRack(Player player) {
            var party = player.CurrentParty;
            var usedTiles = player.UsedRackTiles();
            var rackRefill = party.Game.TakeFromBag(usedTiles).ToList();
            for(var i = 0; i < player.Rack.Length; i++) {
                if(player.Rack[i] == null) {
                    player.Rack[i] = rackRefill[0];
                    rackRefill.RemoveAt(0);
                }
            }
            player.PartyGameCallbackChannel.UpdatePlayerRack(player.Rack);
        }

        public void ConnectPartyGame(string playerSessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var player = Players.Find(p => p.SessionId == playerSessionId);
            if(player != null && player.CurrentParty != null) {
                player.PartyGameCallbackChannel = currentCallbackChannel;
                var party = player.CurrentParty;
                CreatePlayerRack(player);
                UpdatePlayerGame(player);
                RefreshGameTilesLeftCount(party);
                if(GamePlayersAreReady(party)) {
                    SetRandomTurn(party);
                }
            }
        }

        public void EndTurn() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null && currentPlayer.CurrentParty != null) {
                RefillPlayerRack(currentPlayer);
                var party = currentPlayer.CurrentParty;
                var nextPlayerTurn = party.NextTurn();
                foreach(var p in party.Players) {
                    p.PartyGameCallbackChannel.UpdateBagTilesLeft(party.Game.Bag.Count);
                    p.PartyGameCallbackChannel.UpdatePlayerTurn(nextPlayerTurn);
                }
            }
        }

        public void PassTurn() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null && currentPlayer.CurrentParty != null) {
                var party = currentPlayer.CurrentParty;
                var nextPlayerTurn = party.NextTurn();
                foreach(var p in party.Players) {
                    p.PartyGameCallbackChannel.UpdatePlayerTurn(nextPlayerTurn);
                }
            }
        }

        public void PlaceTile(int rackTileIndex, int x, int y) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var currentPlayer = Players.Find(p => p.PartyGameCallbackChannel == currentCallbackChannel);
            if(currentPlayer != null && currentPlayer.CurrentParty != null) {
                var party = currentPlayer.CurrentParty;
                if(party.Players[party.CurrentPlayerTurn] != currentPlayer) {
                    return;
                }
                var tile = currentPlayer.PopTileFromRack(rackTileIndex);
                if(!tile.HasValue) {
                    return;
                }
                var points = party.Game.PlaceTile(tile.Value, x, y);
                currentPlayer.Score += points;
                UpdatePlayersGame(party);
            }
        }
    }
}
