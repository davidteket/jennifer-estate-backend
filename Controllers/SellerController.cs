using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using DunakanyarHouseIngatlan.DataAccess;
using DunakanyarHouseIngatlan.DataAccess.Entities;
using DunakanyarHouseIngatlan.Models;
using DunakanyarHouseIngatlan.Services;

namespace DunakanyarHouseIngatlan.Controllers
{
    public class Seller : Controller
    {
        private IRepository _repo;
        private Serializer _serializer;

        public Seller(IRepository implementation)
        {
            _serializer = new Serializer();
            _repo = implementation;
        }

        #region WebAPI POST

        [HttpPost]
        // Ingatlan értékesítési szándék rögzítése ügyfél részéről.
        //
        public string EstateSellRequest()
        {
            var response = new ItemPostedResultModel();

            string data = _serializer.GetRequestContent(this.HttpContext);
            var clientRequest = new ClientRequest();

            if (data != null)
            {
                clientRequest = (ClientRequest) JsonConvert.DeserializeObject(data, typeof(ClientRequest));
                _repo.AddClientRequest(clientRequest);

                response.Message = "Az értékesítési szándék rögzítésre került.";
                response.Success = true;
            }

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}