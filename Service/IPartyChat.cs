using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IPartyChatCallback))]
    public interface IPartyChat {
        [OperationContract(IsOneWay = true)]
        void ConnectPartyChat(string sessionId);

        [OperationContract(IsOneWay = true)]
        void Say(string message);
    }

    public interface IPartyChatCallback {
        [OperationContract]
        void Receive(Player sender, string message);
    }
}
