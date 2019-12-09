using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using backend.DataAccess;
using backend.DataAccess.Entities;
using backend.Models;

namespace backend.Controllers
{
    public class Seller : Controller
    {
        private IRepository repo = new Repository();

        #region WebAPI POST

        [HttpPost]
        // Ingatlan értékesítési szándék rögzítése ügyfél részéről.
        //
        public string EstateSellRequest()
        {
            var response = new ItemPostedResultModel();

            int length = (int) HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferTask = HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferTask.Wait();

            string data = null;
            var clientRequest = new ClientRequest();

            if (bufferTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                clientRequest = (ClientRequest) JsonConvert.DeserializeObject(data, typeof(ClientRequest));

                repo.AddClientRequest(clientRequest);
                response.Success = true;
                response.Message = "Az értékesítési szándék rögzítésre került.";
            }

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}