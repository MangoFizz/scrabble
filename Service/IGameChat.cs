using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Service {
    [DataContract]
    public class ChatUser {
        [DataMember]
        public String username { get; set; }
        
        [IgnoreDataMember]
        public IGameChatCallback callbackChannel { get; set; }
    }

    [ServiceContract(CallbackContract = typeof(IGameChatCallback))]
    public interface IGameChat {
        [OperationContract(IsOneWay = true)]
        void say(String message);

        [OperationContract(IsOneWay = true)]
        void whisper(String user, String message);
        
        [OperationContract(IsOneWay = true)]
        void join(String user);

        [OperationContract(IsOneWay = true)]
        void leave();
    }

    public interface IGameChatCallback {
        [OperationContract(IsOneWay = true)]
        void Receive(ChatUser sender, String message);

        [OperationContract(IsOneWay = true)]
        void ReceiveWhisper(ChatUser sender, String message);

        [OperationContract(IsOneWay = true)]
        void UserEnter(ChatUser person);

        [OperationContract]
        void UserJoinResponse(ChatUser[] users);

        [OperationContract(IsOneWay = true)]
        void UserLeave(ChatUser person);
    }
}
