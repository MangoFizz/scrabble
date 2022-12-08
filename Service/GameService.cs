using Core;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Service {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class GameService { }

    public partial class GameService : IPlayerManager {
        private List<Player> Players = new List<Player>();

        public void Login(string nickname, string password) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var result = PlayerManager.AuthenticatePlayer(nickname, password);
            if(result == PlayerManager.PlayerAuthResult.Success) {
                using(ScrabbleEntities context = new ScrabbleEntities()) {
                    // Fetch player data
                    var playerData = PlayerManager.GetPlayerData(nickname);
                    var player = new Player(playerData) {
                        PlayerManagerCallbackChannel = currentCallbackChannel
                    };

                    // Fetch player friend list
                    var playerFriendsData = PlayerManager.GetPlayerFriendsData(nickname);
                    player.Friends = playerFriendsData.Select(data => new Player(data)).ToList();

                    // Answer to client
                    Players.Add(player);
                    currentCallbackChannel.LoginResponseHandler(result, player, player.SessionId);

                    // Notify friends that player is online
                    foreach(var friend in player.Friends) {
                        var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                        if(connectedPlayer != null) {
                            connectedPlayer.PlayerManagerCallbackChannel.FriendConnect(player);
                        }
                    }
                }
            }
            else {
                currentCallbackChannel.LoginResponseHandler(result, null, null);
            }
        }

        public void Logout() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(data => data.PlayerManagerCallbackChannel == currentCallbackChannel);

            // Notify friends that player is offline
            foreach(var friend in player.Friends) {
                var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                if(connectedPlayer != null) {
                    connectedPlayer.PlayerManagerCallbackChannel.FriendDisconnect(player);
                }
            }

            Players.Remove(player);
        }

        public void RegisterPlayer(string nickname, string password, string email) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var result = PlayerManager.RegisterPlayer(nickname, password, email);
            currentCallbackChannel.RegisterPlayerResponseHandler(result);
        }

        public void SendFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(data => data.PlayerManagerCallbackChannel == currentCallbackChannel);

            // Send it!!
            var result = PlayerManager.RequestFriendship(player.Nickname, nickname);

            // If success, send friend request notification to the other player if is connected
            if(result == PlayerManager.PlayerFriendRequestResult.Success) {
                var receiver = Players.Find(p => p.Nickname == nickname);
                if(receiver != null) {
                    receiver.PlayerManagerCallbackChannel.ReceiveFriendRequest(player);
                }
            }

            // Answer to client
            currentCallbackChannel.SendFriendRequestResponseHandler(result);
        }

        public void AcceptFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            var result = PlayerManager.AnswerFriendshipRequest(currentPlayer.Nickname, nickname, true);

            // Send notification to players
            if(result == PlayerManager.PlayerFriendshipRequestAnswer.Success) {
                var senderPlayer = Players.Find(p => p.Nickname == nickname);
                bool senderIsConnected = senderPlayer != null;
                if(senderPlayer != null) {
                    senderPlayer.PlayerManagerCallbackChannel.ReceiveFriendAdd(currentPlayer);
                    senderPlayer.PlayerManagerCallbackChannel.FriendConnect(currentPlayer);
                }
                else {
                    var senderPlayerData = PlayerManager.GetPlayerData(nickname);
                    senderPlayer = new Player(senderPlayerData);
                }

                currentPlayer.PlayerManagerCallbackChannel.ReceiveFriendAdd(senderPlayer);
                currentPlayer.Friends.Add(senderPlayer);

                if(senderIsConnected) {
                    currentPlayer.PlayerManagerCallbackChannel.FriendConnect(senderPlayer);
                }
            }
        }

        public void DeclineFriendRequest(string nickname) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            var result = PlayerManager.AnswerFriendshipRequest(player.Nickname, nickname, false);
        }

        public void GetFriendList() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            var playerFriendsData = PlayerManager.GetPlayerFriendsData(player.Nickname);
            var friends = playerFriendsData.Select(data => new Player(data)).ToList();

            // Set friends status
            for(var i = 0; i < friends.Count(); i++) {
                var friend = friends[i];
                var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                if(connectedPlayer != null) {
                    if(connectedPlayer.CurrentParty != null) {
                        friend.status = PlayerStatus.InGame;
                    }
                    else {
                        friend.status = PlayerStatus.Online;
                    }
                }
            }

            currentCallbackChannel.GetFriendListResponseHandler(friends.ToArray());
        }

        public void GetFriendRequests() {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var currentPlayer = Players.Find(p => p.PlayerManagerCallbackChannel == currentCallbackChannel);
            var playerFriendRequestsData = PlayerManager.GetPrendingFriendRequest(currentPlayer.Nickname);
            var friendRequests = playerFriendRequestsData.Select(data => new Player(data));
            currentCallbackChannel.GetFriendRequestsResponseHandler(friendRequests.ToArray());
        }
    }

    public partial class GameService : IPartyChat {
        public void Connect(string sessionId) {
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
        public void Subscribe(string sessionId) {
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
                currentPlayer.CurrentParty = new Party() {
                    Leader = currentPlayer,
                    Players = new List<Player>() { currentPlayer }
                };
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
                        currentPlayer.CurrentParty = party;
                        currentPlayer.PartyManagerCallbackChannel = currentCallbackChannel;
                        currentCallbackChannel.AcceptInvitationCallback(party);

                        party.Players.Add(currentPlayer);
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceivePartyPlayerJoin(currentPlayer);
                        }
                    }
                }
            }
        }

        public void InvitePlayer(Player player) {
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
                    foreach(var p in party.Players) {
                        p.PartyManagerCallbackChannel.ReceivePartyPlayerLeave(player);
                    }

                    // Notify leadership transfer if player was the leader
                    if(player.Nickname == party.Leader.Nickname) {
                        if(party.Players.Count() > 0) {
                            party.Leader = party.Players[0];
                            foreach(var p in party.Players) {
                                p.PartyManagerCallbackChannel.ReceivePartyLeaderTransfer(party.Leader);
                            }
                        }
                    }

                    // Notify friends that player is online
                    foreach(var friend in player.Friends) {
                        var connectedPlayer = Players.FirstOrDefault(p => p.Nickname == friend.Nickname);
                        if(connectedPlayer != null) {
                            connectedPlayer.PlayerManagerCallbackChannel.FriendConnect(player);
                        }
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
                        currentPlayer.PartyManagerCallbackChannel.StartGameCallback(GameStartResult.Success);

                        var game = new Game(language);
                        party.Game = game;
                        party.TimeLimitMins = timeLimitMins;
                        foreach(var p in party.Players) {
                            p.PartyManagerCallbackChannel.ReceiveGameStart();
                        }

                        party.Timer = new System.Threading.Timer((o) => { }, null, 0, timeLimitMins * 60 * 1000);
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
    }

    public partial class GameService : IPartyGame {
        public void ConnectPartyGame(string playerSessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyGameCallback>();
            var player = Players.Find(p => p.SessionId == playerSessionId);
            if(player != null && player.CurrentParty != null) {
                player.PartyGameCallbackChannel = currentCallbackChannel;

                var party = player.CurrentParty;
                var game = party.Game;
                player.Rack = game.TakeFromBag();
                currentCallbackChannel.UpdateBoard(game.GetJaggedBoard());
                currentCallbackChannel.UpdatePlayerRack(player.Rack);

                foreach(var p in party.Players) {
                    if(p.PartyGameCallbackChannel != null) {
                        p.PartyGameCallbackChannel.UpdateBagTilesLeft(game.Bag.Count);
                    }
                }
            }
        }

        public void EndTurn() {
            throw new NotImplementedException();
        }

        public void PassTurn() {
            throw new NotImplementedException();
        }

        public void PlaceTile(Game.Tile tile, int x, int y) {
            throw new NotImplementedException();
        }
    }

}
