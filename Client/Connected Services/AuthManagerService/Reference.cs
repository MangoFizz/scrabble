﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client.AuthManagerService {
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Authenticator.UserAuthResult", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum AuthenticatorUserAuthResult : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InvalidCredentials = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IncorrectPassword = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        DatabaseError = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Authenticator.UserResgisterResult", Namespace="http://schemas.datacontract.org/2004/07/Core")]
    public enum AuthenticatorUserResgisterResult : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UserAlreadyExists = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        DatabaseError = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthManagerService.IAuthManager", CallbackContract=typeof(Client.AuthManagerService.IAuthManagerCallback))]
    public interface IAuthManager {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAuthManager/login")]
        void login(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAuthManager/login")]
        System.Threading.Tasks.Task loginAsync(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAuthManager/registerUser")]
        void registerUser(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAuthManager/registerUser")]
        System.Threading.Tasks.Task registerUserAsync(string username, string password);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthManagerCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthManager/loginResponse", ReplyAction="http://tempuri.org/IAuthManager/loginResponseResponse")]
        void loginResponse(Client.AuthManagerService.AuthenticatorUserAuthResult loginResult);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthManager/registerUserResponse", ReplyAction="http://tempuri.org/IAuthManager/registerUserResponseResponse")]
        void registerUserResponse(Client.AuthManagerService.AuthenticatorUserResgisterResult registrationResult);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthManagerChannel : Client.AuthManagerService.IAuthManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthManagerClient : System.ServiceModel.DuplexClientBase<Client.AuthManagerService.IAuthManager>, Client.AuthManagerService.IAuthManager {
        
        public AuthManagerClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public AuthManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public AuthManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AuthManagerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AuthManagerClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void login(string username, string password) {
            base.Channel.login(username, password);
        }
        
        public System.Threading.Tasks.Task loginAsync(string username, string password) {
            return base.Channel.loginAsync(username, password);
        }
        
        public void registerUser(string username, string password) {
            base.Channel.registerUser(username, password);
        }
        
        public System.Threading.Tasks.Task registerUserAsync(string username, string password) {
            return base.Channel.registerUserAsync(username, password);
        }
    }
}