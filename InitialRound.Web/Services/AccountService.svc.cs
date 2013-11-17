using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.BusinessLogic.Properties;
using InitialRound.Models.Contexts;
using InitialRound.Web.Classes.AccountService;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic.Enums;
using InitialRound.BusinessLogic;
using System.Web;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class AccountService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public LoginResponse Login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();

            if (string.IsNullOrEmpty(request.Username))
            {
                throw new WebFaultException<string>("Username must not be empty.", System.Net.HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                throw new WebFaultException<string>("Password must not be empty.", System.Net.HttpStatusCode.BadRequest);
            }

            try
            {
                AuthToken token = UserController.ValidateUser(request.Username, request.Password, true);

                response.AuthToken = UserController.CreateSessionCookie(token);
                response.ExpirySeconds = (int)Settings.Default.SessionExpiryInterval.TotalSeconds;
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, request.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public KeepAliveResponse KeepAlive(KeepAliveRequest request)
        {
            KeepAliveResponse response = new KeepAliveResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                var newAuthToken = UserController.KeepSessionAlive(authToken);

                response.NewAuthToken = UserController.CreateSessionCookie(newAuthToken);
                response.ExpirySeconds = (int)Settings.Default.SessionExpiryInterval.TotalSeconds;
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CreateAntiForgeryTokenResponse CreateAntiForgeryToken(CreateAntiForgeryTokenRequest request)
        {
            CreateAntiForgeryTokenResponse response = new CreateAntiForgeryTokenResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                response.AntiForgeryToken = UserController.NewAntiForgeryToken(authToken.Username);
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            ResetPasswordResponse response = new ResetPasswordResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "User name");

                Common.Helpers.ValidationHelper.ValidateUsername(request.Username);

                UserController.SendPasswordResetEmail(request.Username);
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, string.Empty);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CompletePasswordResetResponse CompletePasswordReset(CompletePasswordResetRequest request)
        {
            CompletePasswordResetResponse response = new CompletePasswordResetResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "User name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.NewPassword, "New Password");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.NewPassword, "New Password", Constants.MaxPasswordLength);

                Common.Helpers.ValidationHelper.ValidateUsername(request.Username);
                ValidationHelper.ValidatePassword(request.NewPassword);

                ResetPasswordToken token = ResetPasswordToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                Common.Helpers.ValidationHelper.Assert(token.Username.Equals(request.Username), "Invalid input.");
                Common.Helpers.ValidationHelper.Assert(DateTime.Now < token.ExpiresOn, "Password reset request has expired.");

                DbContext context = DataController.CreateDbContext();
                
                UserController.ChangePassword(token.Username, request.NewPassword);
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, string.Empty);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }
        
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            ChangePasswordResponse response = new ChangePasswordResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.OldPassword, "Old Password");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.NewPassword, "New Password");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.OldPassword, "Old Password", Constants.MaxPasswordLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.NewPassword, "New Password", Constants.MaxPasswordLength);

                Common.Helpers.ValidationHelper.AssertFalse(request.NewPassword.Equals(request.OldPassword), "New Password and old password are equal.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                ValidationHelper.ValidatePassword(request.NewPassword);

                UserController.ChangePassword(authToken.Username, request.OldPassword, request.NewPassword);
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}
