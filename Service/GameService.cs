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
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var result = PlayerManager.AuthenticatePlayer(nickname, password);
            if(result == PlayerManager.PlayerAuthResult.Success) {
                using(Scrabble99Entities context = new Scrabble99Entities()) {
                    var playerData = context.players.First(p => p.Nickname == nickname);
                    var player = new Player() {
                        Nickname = playerData.Nickname,
                        Avatar = playerData.Avatar,
                        PlayerManagerCallbackChannel = callbackChannel
                    };

                    var playerFriendsData = PlayerManager.GetPlayerFriendsList(nickname);
                    player.Friends = playerFriendsData.Select(f => new Player() {
                        Nickname = f.Nickname,
                        Avatar = f.Avatar
                    }).ToList();

                    Players.Add(player);
                    callbackChannel.LoginResponseHandler(result, player);
                }
            }
            else {
                callbackChannel.LoginResponseHandler(result, null);
            }
        }

        public void Logout() {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);
            Players.Remove(player);
        }

        public void RegisterPlayer(string nickname, string password, string email) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var result = PlayerManager.RegisterPlayer(nickname, password, email);
            callbackChannel.RegisterPlayerResponseHandler(result);
        }

        public void SendFriendRequest(string nickname) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);

            var result = PlayerManager.RequestFriendship(player.Nickname, nickname);
            callbackChannel.SendFriendRequestResponseHandler(result);
        }

        public void AcceptFriendRequest(string nickname) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);
            var result = PlayerManager.AnswerFriendshipRequest(player.Nickname, nickname, true);
            player.Friends.Add(Players.Find(p => p.Nickname == nickname));
        }

        public void DeclineFriendRequest(string nickname) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);
            var result = PlayerManager.AnswerFriendshipRequest(player.Nickname, nickname, false);
        }

        public void GetFriendList() {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);
            var playerFriendsData = PlayerManager.GetPlayerFriendsList(player.Nickname);
            var friends = playerFriendsData.Select(f => new Player() {
                Nickname = f.Nickname,
                Avatar = f.Avatar
            });
            callbackChannel.GetFriendListResponseHandler(friends.ToArray());
        }

        public void GetFriendRequests() {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerManagerCallback>();
            var player = Players.Find(p => p.PlayerManagerCallbackChannel == callbackChannel);
            var playerFriendRequestsData = PlayerManager.GetPrendingFriendRequest(player.Nickname);
            var friendRequests = playerFriendRequestsData.Select(f => new Player() {
                Nickname = f.Nickname,
                Avatar = f.Avatar
            });
            callbackChannel.GetFriendRequestsResponseHandler(friendRequests.ToArray());
        }
    }

    public partial class GameService : IPartyChat {
        public void Say(string message) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var player = Players.FirstOrDefault(u => u.PartyChatCallbackChannel == callbackChannel);
            if(player != null) {
                foreach(var p in Players) {
                    p.PartyChatCallbackChannel.Receive(player, message);
                }
            }
        }

        public void Whisper(Player receiver, string message) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IPartyChatCallback>();
            var player = Players.FirstOrDefault(u => u.PartyChatCallbackChannel == callbackChannel);
            if(player != null) {
                receiver.PartyChatCallbackChannel.ReceiveWhisper(player, message);
            }
        }
    }
}
