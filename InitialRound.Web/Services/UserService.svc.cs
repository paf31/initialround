using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.BusinessLogic.Properties;
using InitialRound.Models.Contexts;
using InitialRound.Models.Schema.dbo;
using InitialRound.Web.Classes.UserService;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CreateUserResponse CreateUser(CreateUserRequest request)
        {
            CreateUserResponse response = new CreateUserResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.FirstName, "First Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.LastName, "Last Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "User Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.EmailAddress, "Email Address");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Password, "Password");

                Common.Helpers.ValidationHelper.ValidateUsername(request.Username);

                Common.Helpers.ValidationHelper.ValidateStringLength(request.FirstName, "First Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.LastName, "Last Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateMinStringLength(request.Username, "User Name", Constants.MinUsernameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Username, "User Name", Constants.MaxUsernameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.EmailAddress, "Email Address", Constants.MaxUsernameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Password, "Password", Constants.MaxPasswordLength);

                Common.Helpers.ValidationHelper.ValidateEmailAddress(request.EmailAddress);
                ValidationHelper.ValidatePassword(request.Password);

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                UserController.CreateUser(request.Username, request.Password, request.EmailAddress, request.FirstName, request.LastName, authToken, false);
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
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetUserResponse GetUser(GetUserRequest request)
        {
            GetUserResponse response = new GetUserResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "User Name");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context = DataController.CreateDbContext();

                string usernameString = request.Username;

                User user = context.Users.FirstOrDefault(u => u.ID == usernameString);

                response.FirstName = user.FirstName;
                response.LastName = user.LastName;
                response.EmailAddress = user.EmailAddress;
                response.CreatedDate = user.CreatedDate.ToShortDateString();
                response.LastUpdatedDate = user.LastUpdatedDate.ToShortDateString();
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
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetUsersResponse ListUsers(GetUsersRequest request)
        {
            GetUsersResponse response = new GetUsersResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Name, "Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Username, "Username", Constants.MaxUsernameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.EmailAddress, "Email Address", Constants.MaxEmailAddressLength);

                Common.Helpers.ValidationHelper.Assert(request.StartAt < 1E4, "StartAt is too large.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context = DataController.CreateDbContext();

                IQueryable<User> users = context.Users;

                if (!string.IsNullOrEmpty(request.Username))
                {
                    users = users.Where(a => a.ID.Contains(request.Username));
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    users = users.Where(a => (a.FirstName + " " + a.LastName).Contains(request.Name));
                }

                if (!string.IsNullOrEmpty(request.EmailAddress))
                {
                    users = users.Where(a => a.EmailAddress.Contains(request.EmailAddress));
                }

                users = users.OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Skip(request.StartAt)
                    .Take(InitialRound.Web.Properties.Settings.Default.PageSize);

                response.Results = users.AsEnumerable().Select(u => new GetUsersResponseItem
                {
                    Name = u.FirstName + " " + u.LastName,
                    Username = u.ID,
                    EmailAddress = u.EmailAddress,
                    IsAdmin = u.IsAdmin,
                    LastUpdatedDate = u.LastUpdatedDate.ToShortDateString(),
                    CreatedDate = u.CreatedDate.ToShortDateString()
                }).ToArray();
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
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public EditUserResponse EditUser(EditUserRequest request)
        {
            EditUserResponse response = new EditUserResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "Username");

                Common.Helpers.ValidationHelper.ValidateRequiredField(request.FirstName, "First Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.LastName, "Last Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "User Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.EmailAddress, "Email Address");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.FirstName, "First Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.LastName, "Last Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Username, "User Name", Constants.MaxUsernameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.EmailAddress, "Email Address", Constants.MaxUsernameLength);

                Common.Helpers.ValidationHelper.ValidateEmailAddress(request.EmailAddress);

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                UserController.UpdateUser(request.Username, request.FirstName, request.LastName, request.EmailAddress, authToken);
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
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteUser(DeleteUserRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Username, "Username");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Username, "Username", Constants.MaxUsernameLength);

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                UserController.DeleteUser(request.Username, authToken);
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
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
