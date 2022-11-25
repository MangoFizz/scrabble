using Core;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class GameService { }

    public partial class GameService : IPlayerManager {
        private List<Player> Players = new List<Player>();

        public void Login(string nickname, string password) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var result = PlayerManager.AuthenticatePlayer(nickname, password);
            if(result == PlayerManager.PlayerAuthResult.Success) {
                using(Scrabble99Entities context = new Scrabble99Entities()) {
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
                    currentCallbackChannel.LoginResponseHandler(result, player);

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
                currentCallbackChannel.LoginResponseHandler(result, null);
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

    public partial class GameService : IPartyMananger {
        public void CreateParty(string sessionId) {
            var currentCallbackChannel = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            var player = Players.Find(p => p.SessionId == sessionId);
            if(player != null) {
                player.PartyCallbackChannel = currentCallbackChannel;
                player.CurrentParty = new Party() {
                    Leader = player
                };
                currentCallbackChannel.CreatePartyCallback(player.CurrentParty);
            }
        }

        public void AcceptInvitation(Player player) {
            throw new NotImplementedException();
        }

        public void CancelGame() {
            throw new NotImplementedException();
        }

        public void DeclineInvitation(Player player) {
            throw new NotImplementedException();
        }

        public void InvitePlayer(Player player) {
            var targetPlayer = Players.FirstOrDefault(u => u.Nickname == player.Nickname);
            //targetPlayer.PartyCallbackChannel.ReceiveInvitation()
        }

        public void KickPlayer(Player player) {
            throw new NotImplementedException();
        }

        public void LeaveParty() {
            throw new NotImplementedException();
        }

        public void StartGame() {
            throw new NotImplementedException();
        }

        public void TransferLeadership(Player player) {
            throw new NotImplementedException();
        }
    }

}
