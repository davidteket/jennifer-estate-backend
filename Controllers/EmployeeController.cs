using DunakanyarHouseIngatlan.Services;
using DunakanyarHouseIngatlan.DataAccess.Entities.Identity;
using DunakanyarHouseIngatlan.DataAccess;
using DunakanyarHouseIngatlan.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using PasswordGenerator;

using System.Collections.Generic;
using System.Linq;

namespace DunakanyarHouseIngatlan.Controllers
{
    public class EmployeeController : Controller
    {
        private IRepository _repo = new Repository();
        private Serializer _serializer = new Serializer();
        private Email _email = new Email();

        public EmployeeController(IRepository implementation)
        {
            _serializer = new Serializer();
            _email = new Email();
            _repo = implementation;
        }

        #region Util

        // Felhasználói név generálás.
        //
        private string GenerateUserName(string firstName, string lastName)
        {
            string result = null;
            int ext = 1;

            byte[] firstNameBuffer;
            firstNameBuffer = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(firstName);
            firstName = System.Text.Encoding.UTF8.GetString(firstNameBuffer);

            byte[] lastNameBuffer;
            lastNameBuffer = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(lastName);
            lastName = System.Text.Encoding.UTF8.GetString(lastNameBuffer);

            while (true) 
            {
                string toCheck = firstName + "." + lastName + ext.ToString();
                var getUserTask = Startup.UserManager.FindByNameAsync(toCheck);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null)
                    ++ext;
                else 
                {
                    byte[] usernameBuffer;
                    usernameBuffer = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(toCheck);
                    result = System.Text.Encoding.UTF8.GetString(usernameBuffer);
                    break;
                } 
            }

            return result.ToLower();
        }

        #endregion

        #region api/GET


        // Munkatársak.
        //
        [HttpGet]
        public string Employees()
        {
            string response = null;

            IList<EmployeeOverviewViewModel> overviews = new List<EmployeeOverviewViewModel>();
            IList<User> users = new List<User>();

            var getUsersTask = Startup.UserManager.GetUsersInRoleAsync("employee");
            getUsersTask.Wait();

            if (getUsersTask.IsCompletedSuccessfully && getUsersTask.Result != null) 
            {
                users = getUsersTask.Result;
                foreach (User user in users) 
                {
                    var overview = new EmployeeOverviewViewModel();
                
                    overview.EmployeeId = user.Id;
                    overview.FirstName = user.FirstName;
                    overview.LastName = user.LastName;
                    overview.ProfilePictureId = "TODO";

                    overviews.Add(overview);
                }

                response = JsonConvert.SerializeObject(overviews);
            }
            else
                response = _serializer.GetServerLogMessage("EmployeeRetrievalError");

            return response;
        }

        // Munkatárs-profil részletek.
        //
        [HttpGet] 
        public string Details(string employeeId)
        {
            string response = null;

            if (employeeId == null)
                response = _serializer.GetServerLogMessage("EmployeeIdIncorrectError");
            else 
            {
                var getUserTask = Startup.UserManager.FindByIdAsync(employeeId);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) 
                {
                    var user = getUserTask.Result;
                    var details = new EmployeeDetailsViewModel();

                    details.EmployeeId = user.Id;
                    details.FirstName = user.FirstName;
                    details.LastName = user.LastName;
                    details.UserName = user.UserName;
                    details.Phone = user.PhoneNumber;
                    details.Email = user.Email;
                    details.Description = user.Description;
                    details.AdvertisementCount = _repo.GetAdvertisements().Where(a => a.AdvertiserId == user.Id).Count();

                    var image = _repo.GetProfilePictureId(user.Id);

                    if (image == null)
                        details.ProfilePictureId = _serializer.GetDefaults("Picture");
                    else
                        details.ProfilePictureId = image.Id + image.Extension;

                    var getUserRoleTask = Startup.UserManager.GetRolesAsync(user);
                    getUserRoleTask.Wait();

                    if (getUserRoleTask.IsCompletedSuccessfully && getUserRoleTask.Result != null)
                        details.EmployeeRoles = getUserRoleTask.Result.ToList();                    

                    response = JsonConvert.SerializeObject(details);
                }
                else
                    response = _serializer.GetServerLogMessage("EmployeeRetrievalError");
            }

            return response;
        }

        // Munkatárs azonosító.
        //
        [HttpGet]
        public string GetCurrentUserId()
        {
            var response = new ItemPostedResultModel();

            var getUserTask = Startup.UserManager.GetUserAsync(User);
            getUserTask.Wait();

            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) 
            {
                response.ItemId = getUserTask.Result.Id;
                response.Message = _serializer.GetServerLogMessage("EmployeeIdRetrieved");
                response.Success = true;
            }
            else
                response.Message = _serializer.GetServerLogMessage("EmployeeRetrievalError");

            return JsonConvert.SerializeObject(response);
        }

        // Jogosultságok.
        //
        [HttpGet]
        public string GetRoles()
        {
            var response = new List<string>();

            if (Startup.SignInManager.IsSignedIn(User))
                response = Startup.RoleManager.Roles.Select(r => r.Name).ToList();

            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region api/POST

        // Új jogosultság.
        //
        [HttpPost]
        public string NewRole(string roleTitle)
        {
            var response = new ItemPostedResultModel();

            bool allowed = false;

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = _serializer.GetServerLogMessage("AuthenticationRequiredError");
            else 
            {
                string username = this.User.Identity.Name;
                var getUserTask = Startup.UserManager.FindByNameAsync(username);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) {
                    var getRolesTask = Startup.UserManager.GetRolesAsync(getUserTask.Result);
                    getRolesTask.Wait();

                    if (getRolesTask.IsCompletedSuccessfully && getRolesTask.Result != null) {
                        List<string> roles = getRolesTask.Result.ToList();
                        allowed = roles.Any(r => r.ToUpper() == Startup.DeveloperRole.ToUpper());
                    }
                }
            }

            if (allowed == false)
                response.Message = _serializer.GetServerLogMessage("RootRoleRequiredError");

            var role = new UserRole();
            if (roleTitle == null)
                response.Message = _serializer.GetServerLogMessage("RoleTitleUnavailableError");
            else 
                role.Name = roleTitle;
            
            var addRoleTask = Startup.RoleManager.CreateAsync(role);
            addRoleTask.Wait();

            if (addRoleTask.IsCompletedSuccessfully && addRoleTask.Result.Succeeded)
            {
                response.Message = _serializer.GetServerLogMessage("RoleTitleCreated");
                response.Success = addRoleTask.Result.Succeeded;
                response.ItemId = role.Name;
            }
            else 
                response.Message = _serializer.GetServerLogMessage("RoleTitleError");

            return JsonConvert.SerializeObject(response);
        }

        // Munkatárs regisztrálás.
        //
        [HttpPost]
        public string Registration()
        {
            var response = new ItemPostedResultModel();

            bool allowed = false;

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = _serializer.GetServerLogMessage("AuthenticationRequiredError");
            else 
            {
                string username = this.User.Identity.Name;
                var getUserTask = Startup.UserManager.FindByNameAsync(username);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) {
                    var getRolesTask = Startup.UserManager.GetRolesAsync(getUserTask.Result);
                    getRolesTask.Wait();

                    if (getRolesTask.IsCompletedSuccessfully && getRolesTask.Result != null) {
                        List<string> roles = getRolesTask.Result.ToList();
                        allowed = roles.Any(r => r.ToUpper() == Startup.DeveloperRole.ToUpper());
                    }
                }
            }

            if (allowed == false)
                response.Message = _serializer.GetServerLogMessage("RootRoleToUserRequiredError");
            
            var user = new DataAccess.Entities.Identity.User();
            RegistrationViewModel registration = null;
            string data = _serializer.GetRequestContent(this.HttpContext);

            if (data != null)                
                registration = (RegistrationViewModel) JsonConvert.DeserializeObject(data, typeof(RegistrationViewModel));

            var passGenerator = new Password();
            string tempPassword = passGenerator.Next();;
            string userName = GenerateUserName(registration.FirstName, registration.LastName);

            user.Description = registration.Description;
            user.Email = registration.Email;
            user.FirstName = registration.FirstName;
            user.LastName = registration.LastName;
            user.PhoneNumber = registration.Phone;
            user.UserName = userName;

            var createUserTask = Startup.UserManager.CreateAsync(user, tempPassword);
            createUserTask.Wait();

            if (!createUserTask.IsCompletedSuccessfully || !createUserTask.Result.Succeeded) {
                response = new ItemPostedResultModel();
                response.Message = _serializer.GetServerLogMessage("EmployeeError");
            }
            else 
            {
                var getUserTask = Startup.UserManager.FindByNameAsync(user.UserName);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) 
                {
                    User newUser = getUserTask.Result;
                    response.ItemId = newUser.Id;
                    response.Message = _serializer.GetServerLogMessage("EmployeeCreated");

                    var addToRoleTask = Startup.UserManager.AddToRoleAsync(newUser, registration.RoleTitle);
                    addToRoleTask.Wait();

                    if (addToRoleTask.IsCompletedSuccessfully && addToRoleTask.Result != null) 
                    {
                        bool emailSent = _email.NotifyRegistration(user.Email, string.Concat(user.LastName, " ", user.FirstName), user.UserName, tempPassword, registration.RoleTitle);

                        response.Message += _serializer.GetServerLogMessage("EmployeeReceivedRole").Replace("{0}", registration.RoleTitle);
                        response.Message += response.Success ? _serializer.GetServerLogMessage("EmailNotification") : _serializer.GetServerLogMessage("EmailNotificationError");
                        response.Success = true;
                    }
                    else {
                        response.Message += _serializer.GetServerLogMessage("EmployeeRoleError");
                        response.Success = false;
                    }
                }
            }

            return JsonConvert.SerializeObject(response);
        }

        // Munkatárs beléptetés.
        //
        [HttpPost]
        public string Login()
        {
            var response = new ItemPostedResultModel();

            var credentials = new Login();
            string data = _serializer.GetRequestContent(this.HttpContext);

            if (data != null)
                credentials = (Login) JsonConvert.DeserializeObject(data, typeof(Login));

            var getUserTask = Startup.UserManager.FindByNameAsync(credentials.UserName);
            getUserTask.Wait();

            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null)
            {
                User user = getUserTask.Result;
                var checkPasswordTask = Startup.UserManager.CheckPasswordAsync(user, credentials.Password);
                checkPasswordTask.Wait();

                if (checkPasswordTask.IsCompletedSuccessfully && checkPasswordTask.Result == true)
                {
                    var signInTask = Startup.SignInManager.PasswordSignInAsync(user, credentials.Password, false, false);
                    signInTask.Wait();

                    if (signInTask.IsCompletedSuccessfully)
                    {
                        response.ItemId = user.Id;
                        response.Success = true;
                        response.Message = _serializer.GetServerLogMessage("Authentication");
                    }
                }
                else 
                    response.Message = _serializer.GetServerLogMessage("AuthenticationPasswordError");
            }
            else
                response.Message = _serializer.GetServerLogMessage("AuthenticationUserError");

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs kiléptetés.
        //
        public string Logout()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = _serializer.GetServerLogMessage("AuthenticationRequiredError");

            var signOutTask = Startup.SignInManager.SignOutAsync();
            signOutTask.Wait();

            if (signOutTask.IsCompletedSuccessfully)
                response.Message = _serializer.GetServerLogMessage("AuthenticationLoggedOut");
            else
                response.Message = _serializer.GetServerLogMessage("AuthenticationLoggedOutError");
                
            return JsonConvert.SerializeObject(response);
        }

        // Munkatárs-profil módosítás.
        //
        [HttpPost]
        public string UpdateProfile()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = _serializer.GetServerLogMessage("AuthenticationRequiredError");

            var bio = new EmployeeDetailsViewModel();
            string data = _serializer.GetRequestContent(this.HttpContext);

            if (data != null)
                bio = (EmployeeDetailsViewModel) JsonConvert.DeserializeObject(data, typeof(EmployeeDetailsViewModel));

            var getUserTask = Startup.UserManager.GetUserAsync(this.User);
            getUserTask.Wait();

            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null)
            {
                User user = getUserTask.Result;

                user.FirstName = bio.FirstName;
                user.LastName = bio.LastName;
                user.Description = bio.Description;
                user.PhoneNumber = user.PhoneNumber;

                var updateUserTask = Startup.UserManager.UpdateAsync(user);
                updateUserTask.Wait();

                if (updateUserTask.IsCompletedSuccessfully && updateUserTask.Result.Succeeded) 
                {
                    response.ItemId = user.Id;
                    response.Success = updateUserTask.Result.Succeeded;
                    response.Message = _serializer.GetServerLogMessage("EmployeeProfileUpdated");
                }

            }
            else
                response.Message = _serializer.GetServerLogMessage("EmployeeRetrievalError");
                
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs törlés.
        //
        public string Delete(string employeeId)
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = _serializer.GetServerLogMessage("AuthenticationRequiredError");

            if (employeeId == null)
                response.Message = _serializer.GetServerLogMessage("EmployeeIdIncorrectError");

            var getUserTask = Startup.UserManager.FindByIdAsync(employeeId);
            getUserTask.Wait();
            var user = new User();

            if (!getUserTask.IsCompletedSuccessfully || getUserTask.Result == null)
                response.Message = _serializer.GetServerLogMessage("EmployeeIdRetrievalError");
            else 
            {
                user = getUserTask.Result;
                response.ItemId = user.Id;
            }

            var deleteTask = Startup.UserManager.DeleteAsync(user);
            deleteTask.Wait();

            if (!deleteTask.IsCompletedSuccessfully || !deleteTask.Result.Succeeded)
                response.Message = _serializer.GetServerLogMessage("EmployeeDeleteError");
            else 
            {
                response.Message = _serializer.GetServerLogMessage("EmployeeDelete");
                response.Success = true;
            }

            return JsonConvert.SerializeObject(response);
        }

        #endregion        
    }
}