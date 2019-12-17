using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using backend.Services;
using backend.Models;
using backend.DataAccess;

namespace backend.Controllers
{
    public class Application : Controller
    {
        private IRepository _repo = new Repository();
        private Serializer _serializer = new Serializer();

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
                        // LOG - green
                    }
                    else {
                        response.Success = false;
                        response.Message = _serializer.GetServerLogMessage("ApplicationDataEreaseError");
                        // LOG - red
                    }
                }
                else {
                    response.Success = false;
                    response.Message = _serializer.GetServerLogMessage("EmployeeRoleNotHighEnough");
                    // LOG - yellow
                }
            }
            else {
                response.Success = false;
                response.Message = _serializer.GetServerLogMessage("EmployeeRetrievalError");
                // LOG - red
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}