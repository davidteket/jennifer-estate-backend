using backend.DataAccess.Entities.Identity;
using backend.DataAccess;
using backend.Models;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Net.Mail;

namespace backend.Controllers
{
    public class EmployeeController : Controller
    {
        private IRepository repo = new Repository();

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
            else
                response = "A profil adatok lekérdezése sikertelen volt.";


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
                response = "Nincs megadva azonosító.";
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
                    details.Phone = user.PhoneNumber;
                    details.Email = user.Email;
                    details.Advertisements = null; // TODO
                    details.ProfilePictureId = "TODO";

                    response = JsonConvert.SerializeObject(details);
                }
                else
                    response = "A megadott azonosító nem érvényes.";
            }

            cancelBio:
            return response;
        }

        #endregion

        #region WebAPI POST

        [HttpPost]
        // Új jogosultság.
        //
        public string NewRole(string roleTitle)
        {
            var response = new ItemPostedResultModel();

            var role = new UserRole();

            if (roleTitle == null) {
                response.Message = "Nincs megadva jogosultsági cím.";
                goto cancelNewRole;
            }
            else 
                role.Name = roleTitle;
            
            var addRoleTask = Startup.RoleManager.CreateAsync(role);
            addRoleTask.Wait();

            if (addRoleTask.IsCompletedSuccessfully && addRoleTask.Result.Succeeded)
            {
                response.Message = "A jogosultság létrehozásra került.";
                response.Success = addRoleTask.Result.Succeeded;
                response.ItemId = role.Name;
            }
            else 
                response.Message = "A jogosultság létrehozása sikertelen volt.";

            cancelNewRole:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs regisztrálás.
        //
        public string Registration()
        {
            var response = new ItemPostedResultModel();
            
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

                /*bool hasBeenInvited = repo.CheckInvitationStatus(registration.Invitation);
                if (hasBeenInvited == false) {
                    response.Message = "Az oldalra csak meghívással lehet regisztrálni. A megadott meghívó azonosító érvénytelen.";
                    goto cancelRegistration;
                }*/
            }

            var createUserTask = Startup.UserManager
                                        .CreateAsync(registration.User, registration.Password);
            createUserTask.Wait();

            if (!createUserTask.IsCompletedSuccessfully || !createUserTask.Result.Succeeded) {
                response = new ItemPostedResultModel();
                response.Message = "A felhasználói regisztráció sikertelen volt. Kérlek próbáld meg újra, vagy vedd fel a kapcsolatot az oldal üzemeltetőjével!";
            }
            else 
            {
                var getUserIdTask = Startup.UserManager.GetUserIdAsync(registration.User);
                getUserIdTask.Wait();

                if (getUserIdTask.IsCompletedSuccessfully) 
                {
                    response.ItemId = getUserIdTask.Result;
                    response.Success = createUserTask.Result.Succeeded;
                    response.Message = "A felhasználói regisztráció sikeresen megtörtént.";

                    var getUserTask = Startup.UserManager.FindByIdAsync(getUserIdTask.Result);
                    getUserTask.Wait();

                    if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null) {
                        var addToRoleTask = Startup.UserManager.AddToRoleAsync(getUserTask.Result, "employee");
                        addToRoleTask.Wait();

                        if (addToRoleTask.IsCompletedSuccessfully && addToRoleTask.Result != null)
                            response.Message += "\n A felhasználó dolgozói jogosultságot kapott.";
                        else
                            response.Message += "\n A felhasználó jogosultsági beállítása sikertelen volt.";

                    }

                    //repo.InvalidateInvitationTicket(registration.Invitation.Id);
                }
            }

            //cancelRegistration:
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
                    var signInTask = Startup.SignInManager.PasswordSignInAsync(user, credentials.Password, true, false);
                    signInTask.Wait();

                    if (signInTask.IsCompletedSuccessfully) {
                        response.ItemId = user.Id;
                        response.Success = true;
                        response.Message = "A bejelentkezés sikeresen megtörtént.";
                    }
                }
                else 
                    response.Message = "A megadott jelszó érvénytelen volt. Próbáld meg újra, vagy vedd fel a kapcsolatot az oldal adminisztrátorával.";
            }
            else
                response.Message = "A bejelentkezés sikertelen volt, vagy nem létezik a megadott felhasználó. Próbáld meg újra, vagy vedd fel a kapcsolatot az oldal üzemeltetőjével.";

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs kiléptetés.
        //
        public string Logout()
        {
            var response = new ItemPostedResultModel();

            var signOutTask = Startup.SignInManager.SignOutAsync();
            signOutTask.Wait();

            if (signOutTask.IsCompletedSuccessfully)
                response.Message = "A kijelentkezés megtörtént.";
            else
                response.Message = "Hiba történt kijelentkezéskor.";

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Munkatárs-profil módosítás.
        //
        public string UpdateProfile()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(new System.Security.Claims.ClaimsPrincipal(HttpContext.User.Identity)))
            {
                response.Message = "A művelet elvégzéséhez bejelentkezés szükséges.";
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
                    response.Message = "A felhasználói profil módosítása sikeresen megtörtént.";
                }

            }
            else {
                response.Message = "A kéréshez nem tartozik felhasználó, a módosítás sikertelen volt.";
            }

            cancelUpdateBio:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Meghívó küldés regisztrációhoz.
        public string SendInvitation(string inviteeId, string destination)
        {
            string response = null;

            var client = new SmtpClient(Startup.Email.Host, Startup.Email.Port);
            client.EnableSsl = true;
        
            var company = repo.GetCompanyDetails();

            // TODO:
            //
            //  - Invitation GUID generation (6 digits)
            //  - Invitation expirity generation (24 hours)
            //  - clickable link generation
            //  - log the sender id (user id)
            //
            var message = new MailMessage(company.CompanyEmailAddress, destination, company.InvitationSubject, company.InvitationMessage);
            var emailSenderTask = client.SendMailAsync(message);
            emailSenderTask.Wait();

            if (emailSenderTask.IsCompletedSuccessfully) 
            {
                response = "A meghívó elküldése sikeres volt.";
                repo.AddInvitation(inviteeId);
            }
            else
                response = "Nem sikerült elküldeni a meghívót.";

            return response;
        }

        [HttpPost]
        // Kapcsolatfelvétel.
        //
        public string Contact()
        {
            // TODO

            return null;
        }

        [HttpPost]
        // Email küldés.
        //
        public string EmailSender()
        {
            // TODO

            return null;
        }

        [HttpPost]
        // Munkatárs törlés.
        //
        public string Delete(string employeeId)
        {
            var response = new ItemPostedResultModel();

            if (employeeId == null) {
                response.Message = "Nincs megadva azonosító.";
                goto cancelDelete;
            }

            var getUserTask = Startup.UserManager.FindByIdAsync(employeeId);
            getUserTask.Wait();
            var user = new User();

            if (!getUserTask.IsCompletedSuccessfully || getUserTask.Result == null)
                response.Message = "A megadott azonosítójú munkatárs nem található.";
            else {
                user = getUserTask.Result;
                response.ItemId = user.Id;
            }

            var deleteTask = Startup.UserManager.DeleteAsync(user);
            deleteTask.Wait();

            if (!deleteTask.IsCompletedSuccessfully || !deleteTask.Result.Succeeded)
                response.Message = "A megadott azonosítójú munkatárs törlése sikertelen volt.";
            else {
                response.Message = "A felhasználó törlése sikeresen megtörtént.";
                response.Success = deleteTask.Result.Succeeded;
            }

            cancelDelete:
            return JsonConvert.SerializeObject(response);
        }

        #endregion        
    }
}