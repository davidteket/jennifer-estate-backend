using backend.Services;
using backend.DataAccess.Entities.Identity;
using backend.DataAccess;
using backend.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace backend.Controllers
{
    public class EmployeeController : Controller
    {
        private IRepository _repo = new Repository();
        private Serializer _serialize = new Serializer();
        private Email _email = new Email();

        #region Utility

        // Ideiglenes jelszó generálás.
        //
        private string GeneratePassword()
        {
            string result = null;

            int minLength = 8;
            int maxLength = 16;
            string pool = _serialize.GetDefaults("TmpPassRegularCharPool");
            string spec = _serialize.GetDefaults("TmpPassSpecialCharPool");

            var rnd = new System.Random();
            int length = rnd.Next(minLength, maxLength);

            for (int i = 0; i < length; ++i) {
                int j = rnd.Next(0, pool.Length - 1);
                int k = rnd.Next(0, 1);
                int l = rnd.Next(0, 1);
                int m = rnd.Next(0, 1);

                bool isUpper = k == 1 ? true : false;
                bool addNumber = l == 1 ? true : false;
                int s = rnd.Next(0, spec.Length - 1);

                string chunk = isUpper ? pool[j].ToString().ToUpper() : pool[j].ToString();
                chunk += addNumber ? i.ToString() : "0";
                chunk += spec[s].ToString();

                result += chunk;
            }

            return result;
        }

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

        #region WebAPI GET


        [HttpGet]
        // Munkatársak.
        //
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
                    overview.ApproachType = user.ApproachType;
                    overview.FirstName = user.FirstName;
                    overview.MiddleName = user.MiddleName;
                    overview.LastName = user.LastName;
                    overview.ProfilePictureId = "TODO";

                    overviews.Add(overview);
                }

                response = JsonConvert.SerializeObject(overviews);
            }
            else{
                response = _serialize.GetServerLogMessage("EmployeeRetrievalError");
                // LOG - red
            }

            return response;
        }

        [HttpGet] 
        // Munkatárs-profil részletek.
        //
        public string Details(string employeeId)
        {
            string response = null;

            var details = new EmployeeDetailsViewModel();
            if (employeeId == null) {
                response = _serialize.GetServerLogMessage("EmployeeIdIncorrectError");
                goto cancelBio;
            }
            else {
                var getUserTask = Startup.UserManager.FindByIdAsync(employeeId);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) 
                {
                    var user = new User();
                    user = getUserTask.Result;

                    details.EmployeeId = user.Id;
                    details.ApproachType = user.ApproachType;
                    details.FirstName = user.FirstName;
                    details.MiddleName = user.MiddleName;
                    details.LastName = user.LastName;
                    details.UserName = user.UserName;
                    details.Phone = user.PhoneNumber;
                    details.Email = user.Email;
                    details.Description = user.Description;
                    details.AdvertisementCount = _repo.GetAdvertisements().Where(a => a.AdvertiserId == user.Id).Count();
                    var image = _repo.GetProfilePictureId(user.Id);
                    if (image == null) {
                        details.ProfilePictureId = _serialize.GetDefaults("Picture");
                    }
                    else
                        details.ProfilePictureId = image.Id + image.Extension;

                    var getUserRoleTask = Startup.UserManager.GetRolesAsync(user);
                    getUserRoleTask.Wait();

                    if (getUserRoleTask.IsCompletedSuccessfully && getUserRoleTask.Result != null)
                        details.EmployeeRoles = getUserRoleTask.Result.ToList();
                    

                    response = JsonConvert.SerializeObject(details);
                }
                else {
                    response = _serialize.GetServerLogMessage("EmployeeRetrievalError");
                    // LOG - red
                }
            }

            cancelBio:
            return response;
        }

        [HttpGet]
        // Munkatárs azonosító.
        //
        public string GetCurrentUserId()
        {
            var response = new ItemPostedResultModel();

            var getUserTask = Startup.UserManager.GetUserAsync(User);
            getUserTask.Wait();

            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) {
                response.ItemId = getUserTask.Result.Id;
                response.Message = _serialize.GetServerLogMessage("EmployeeIdRetrieved");
                response.Success = true;

                // LOG - green
            }
            else {
                response.Message = _serialize.GetServerLogMessage("EmployeeRetrievalError");

                // LOG - red
            }

            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]
        // Jogosultságok.
        //
        public string GetRoles()
        {
            var response = new List<string>();

            if (Startup.SignInManager.IsSignedIn(User))
                response = Startup.RoleManager.Roles.Select(r => r.Name).ToList();

            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region WebAPI POST

        [HttpPost]
        // Új jogosultság.
        //
        public string NewRole(string roleTitle)
        {
            var response = new ItemPostedResultModel();

            bool allowed = false;
            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = _serialize.GetServerLogMessage("AuthenticationRequiredError");
                goto cancelNewRole;
            }
            else {
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

            if (allowed == false) {
                response.Message = _serialize.GetServerLogMessage("RootRoleRequiredError");
                goto cancelNewRole;
            }

            var role = new UserRole();

            if (roleTitle == null) {
                response.Message = _serialize.GetServerLogMessage("RoleTitleUnavailableError");
                goto cancelNewRole;
            }
            else 
                role.Name = roleTitle;
            
            var addRoleTask = Startup.RoleManager.CreateAsync(role);
            addRoleTask.Wait();

            if (addRoleTask.IsCompletedSuccessfully && addRoleTask.Result.Succeeded)
            {
                response.Message = _serialize.GetServerLogMessage("RoleTitleCreated");
                response.Success = addRoleTask.Result.Succeeded;
                response.ItemId = role.Name;
            }
            else 
                response.Message = _serialize.GetServerLogMessage("RoleTitleError");

            cancelNewRole:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs regisztrálás.
        //
        public string Registration()
        {
            var response = new ItemPostedResultModel();

            bool allowed = false;
            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = _serialize.GetServerLogMessage("AuthenticationRequiredError");
                goto cancelRegistration;
            }
            else {
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

            if (allowed == false) {
                response.Message = _serialize.GetServerLogMessage("RootRoleToUserRequiredError");
                goto cancelRegistration;
            }
            
            var user = new DataAccess.Entities.Identity.User();

            int length = (int) HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferTask = HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferTask.Wait();
            string data = null;

            RegistrationViewModel registration = null;

            if (bufferTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                registration = (RegistrationViewModel) JsonConvert.DeserializeObject(data, typeof(RegistrationViewModel));
            }

            string tempPassword = GeneratePassword();
            string userName = GenerateUserName(registration.FirstName, registration.LastName);

            user.Description = registration.Description;
            user.Email = registration.Email;
            user.FirstName = registration.FirstName;
            user.MiddleName = registration.MiddleName;
            user.LastName = registration.LastName;
            user.PhoneNumber = registration.Phone;
            user.UserName = userName;

            var createUserTask = Startup.UserManager.CreateAsync(user, tempPassword);
            createUserTask.Wait();

            if (!createUserTask.IsCompletedSuccessfully || !createUserTask.Result.Succeeded) {
                response = new ItemPostedResultModel();
                response.Message = _serialize.GetServerLogMessage("EmployeeError");
            }
            else 
            {
                var getUserTask = Startup.UserManager.FindByNameAsync(user.UserName);
                getUserTask.Wait();

                if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) 
                {
                    User newUser = getUserTask.Result;
                    response.ItemId = newUser.Id;
                    response.Message = _serialize.GetServerLogMessage("EmployeeCreated");

                    var addToRoleTask = Startup.UserManager.AddToRoleAsync(newUser, registration.RoleTitle);
                    addToRoleTask.Wait();

                    if (addToRoleTask.IsCompletedSuccessfully && addToRoleTask.Result != null) {
                        response.Message += _serialize.GetServerLogMessage("EmployeeReceivedRole").Replace("{0}", registration.RoleTitle);
                        response.Success = _email.NotifyRegistration(user.Email, string.Concat(user.LastName, " ", user.FirstName), user.UserName, tempPassword, registration.RoleTitle);
                        response.Message += response.Success ? _serialize.GetServerLogMessage("EmailNotification") : _serialize.GetServerLogMessage("EmailNotificationError");
                    }
                    else {
                        response.Message += _serialize.GetServerLogMessage("EmployeeRoleError");
                        response.Success = false;
                    }
                }
            }

            cancelRegistration:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs beléptetés.
        //
        public string Login()
        {
            var response = new ItemPostedResultModel();

            var credentials = new Login();

            int length = (int) HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferTask = HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferTask.Wait();

            string data = null;

            if (bufferTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                credentials = (Login) JsonConvert.DeserializeObject(data, typeof(Login));
            }

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

                    if (signInTask.IsCompletedSuccessfully) {
                        response.ItemId = user.Id;
                        response.Success = true;
                        response.Message = _serialize.GetServerLogMessage("Authentication");
                    }
                }
                else 
                    response.Message = _serialize.GetServerLogMessage("AuthenticationPasswordError");
            }
            else
                response.Message = _serialize.GetServerLogMessage("AuthenticationUserError");

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs kiléptetés.
        //
        public string Logout()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = _serialize.GetServerLogMessage("AuthenticationRequiredError");
                goto cancelLogout;
            }

            var signOutTask = Startup.SignInManager.SignOutAsync();
            signOutTask.Wait();

            if (signOutTask.IsCompletedSuccessfully)
                response.Message = _serialize.GetServerLogMessage("AuthenticationLoggedOut");
            else
                response.Message = _serialize.GetServerLogMessage("AuthenticationLoggedOutError");

            cancelLogout: 
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs-profil módosítás.
        //
        public string UpdateProfile()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = _serialize.GetServerLogMessage("AuthenticationRequiredError");
                goto cancelUpdateBio;
            }

            var bio = new Bio();

            int length = (int) HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferTask = HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferTask.Wait();

            if (bufferTask.IsCompletedSuccessfully)
            {
                string data = System.Text.Encoding.Default.GetString(buffer);
                bio = (Bio) JsonConvert.DeserializeObject(data, typeof(Bio));
            }

            var getUserTask = Startup.UserManager.GetUserAsync(this.User);
            getUserTask.Wait();


            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null)
            {
                User user = getUserTask.Result;

                user.FirstName = bio.FirstName;
                user.MiddleName = bio.MiddleName;
                user.LastName = bio.LastName;
                user.ApproachType = bio.ApproachType;
                user.Description = bio.Description;
                user.PhoneNumber = user.PhoneNumber;

                var updateUserTask = Startup.UserManager.UpdateAsync(user);
                updateUserTask.Wait();

                if (updateUserTask.IsCompletedSuccessfully && updateUserTask.Result.Succeeded) {
                    response.ItemId = user.Id;
                    response.Success = updateUserTask.Result.Succeeded;
                    response.Message = _serialize.GetServerLogMessage("EmployeeProfileUpdated");
                }

            }
            else {
                response.Message = _serialize.GetServerLogMessage("EmployeeRetrievalError");
            }

            cancelUpdateBio:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs törlés.
        //
        public string Delete(string employeeId)
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = _serialize.GetServerLogMessage("AuthenticationRequiredError");
                goto cancelDelete;
            }

            if (employeeId == null) {
                response.Message = _serialize.GetServerLogMessage("EmployeeIdIncorrectError");
                goto cancelDelete;
            }

            var getUserTask = Startup.UserManager.FindByIdAsync(employeeId);
            getUserTask.Wait();
            var user = new User();

            if (!getUserTask.IsCompletedSuccessfully || getUserTask.Result == null)
                response.Message = _serialize.GetServerLogMessage("EmployeeIdRetrievalError");
            else {
                user = getUserTask.Result;
                response.ItemId = user.Id;
            }

            var deleteTask = Startup.UserManager.DeleteAsync(user);
            deleteTask.Wait();

            if (!deleteTask.IsCompletedSuccessfully || !deleteTask.Result.Succeeded)
                response.Message = _serialize.GetServerLogMessage("EmployeeDeleteError");
            else {
                response.Message = _serialize.GetServerLogMessage("EmployeeDelete");
                response.Success = deleteTask.Result.Succeeded;
            }

            cancelDelete:
            return JsonConvert.SerializeObject(response);
        }

        #endregion        
    }
}