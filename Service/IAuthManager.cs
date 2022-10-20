using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Core;

using static Core.Authenticator;

namespace Service {
    [ServiceContract(CallbackContract = typeof(IAuthManagerCallback))]
    public interface IAuthManager {
        [OperationContract(IsOneWay = true)]
        void login(String username, String password);

        [OperationContract(IsOneWay = true)]
        void registerUser(String username, String password);
    }

    [ServiceContract]
    interface IAuthManagerCallback {
        [OperationContract]
        void loginResponse(UserAuthResult loginResult);

        [OperationContract]
        void registerUserResponse(UserResgisterResult registrationResult);
    }
}
