using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using DunakanyarHouseIngatlan.Services;
using DunakanyarHouseIngatlan.Models;
using DunakanyarHouseIngatlan.DataAccess;

namespace DunakanyarHouseIngatlan.Controllers
{
    public class Application : Controller
    {
        private IRepository _repo;
        private Serializer _serializer;

        public Application(IRepository implementation)
        {
            _serializer = new Serializer();
            _repo = implementation;
        }
        
        // Az alkalmazás  gyári beállításainak visszaállítása valamint a letárolt adatok végleges törlése.
        // Ez a művelet csak root jogosultságú felhasználóval hajtható végre.
        //
        [HttpGet]
        public string FactoryReset()
        {
            var response = new ItemPostedResultModel();

            var getUserTask = Startup.UserManager.GetUserAsync(HttpContext.User);
            getUserTask.Wait();

            if (getUserTask.IsCompletedSuccessfully && getUserTask.Result != null)
            {
                var isAllowedTask = Startup.UserManager.IsInRoleAsync(getUserTask.Result, Startup.DeveloperRole);
                isAllowedTask.Wait();

                if (isAllowedTask.IsCompletedSuccessfully && isAllowedTask.Result == true) {
                    bool success = _repo.DeleteAllEntries();
                    if (success) {
                        response.Success = true;
                        response.Message = _serializer.GetServerLogMessage("ApplicationDataErease");
                    }
                    else {
                        response.Success = false;
                        response.Message = _serializer.GetServerLogMessage("ApplicationDataEreaseError");
                    }
                }
                else {
                    response.Success = false;
                    response.Message = _serializer.GetServerLogMessage("EmployeeRoleNotHighEnough");
                }
            }
            else {
                response.Success = false;
                response.Message = _serializer.GetServerLogMessage("EmployeeRetrievalError");
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}