using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Core;

namespace Service {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AuthManager : IAuthManager {
        public void login(string username, string password) {
            var result = Authenticator.validateUser(username, password);
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IAuthManagerCallback>();
            callbackChannel.loginResponse(result);
        }

        public void registerUser(string username, string password) {
            var result = Authenticator.registerUser(username, password);
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IAuthManagerCallback>();
            callbackChannel.registerUserResponse(result);
        }
    }
}
