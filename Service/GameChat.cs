using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Service {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameChat : IGameChat {
        private static List<ChatUser> users = new List<ChatUser>();

        public void join(string userName) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IGameChatCallback>();
            var chatUser = new ChatUser() { username = userName, callbackChannel = callbackChannel };
            foreach(var user in users) {
                user.callbackChannel.UserEnter(chatUser);
            }
            users.Add(chatUser);
            callbackChannel.UserJoinResponse(users.ToArray());
        }

        public void leave() {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IGameChatCallback>();
            var chatUser = users.FirstOrDefault(u => u.callbackChannel == callbackChannel);
            if(chatUser != null) {
                users.Remove(chatUser);
                foreach(var user in users) {
                    user.callbackChannel.UserLeave(chatUser);
                }
            }
        }

        public void say(string message) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IGameChatCallback>();
            var chatUser = users.FirstOrDefault(u => u.callbackChannel == callbackChannel);
            Console.WriteLine("{0} says: {1}", chatUser.username, message);
            if(chatUser != null) {
                foreach(var user in users) {
                    Console.WriteLine("Sending to {0}", user.username);
                    user.callbackChannel.Receive(chatUser, message);
                }
            }
        }

        public void whisper(string user, string message) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IGameChatCallback>();
            var chatUser = users.FirstOrDefault(u => u.callbackChannel == callbackChannel);
            if(chatUser != null) {
                var targetUser = users.FirstOrDefault(u => u.username == user);
                if(targetUser != null) {
                    targetUser.callbackChannel.ReceiveWhisper(chatUser, message);
                }
            }
        }
    }
}
