using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using backend.DataAccess.Entities;
using backend.Models;
using backend.DataAccess;

namespace backend.Controllers
{
    public class EstateController : Controller
    {
        private IRepository repo = new Repository();

        #region WebAPI GET  

        [HttpGet] 
        //  Adott darabszámú ingatlan lekérés.
        //
        public string GetEstates(int from = 1, int limit = 10)
        {
            var response = new List<EstateOverviewViewModel>();

            List<Estate> estates = repo.GetEstates(from, limit);
            foreach (Estate estate in estates)
            {
                var estateOverview = new EstateOverviewViewModel();

                Advertisement ad = repo.GetAdvertisement(estate.Id);
                Address address = repo.GetAddress(estate.Id);
                GenericImage image = repo.GetEstateThumbnail(estate.Id);

                estateOverview.EstateId = estate.Id;
                estateOverview.Image = image.Id;
                estateOverview.Title = ad.Title;
                estateOverview.Price = estate.Price;
                estateOverview.OfferType = ad.OfferType;
                estateOverview.City = address.City;
                estateOverview.PostCode = address.PostCode;
                estateOverview.Street = address.Street;
                estateOverview.Country = address.Country;

                response.Add(estateOverview);
            }

            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]   
        //  Adott ingatlan lekérés.
        //
        public string Detail(int estateId)
        {
            var response = new EstateDetailsViewModel();

            Estate estate = repo.GetEstate(estateId);
            response.SquareFeet = estate.SquareFeet;
            response.Category = estate.Category;
            response.BuiltAt = estate.BuiltAt;
            response.RefurbishedAt = estate.RefurbishedAt.ToString();
            response.Grade = estate.Grade;
            response.Room = estate.Room;
            response.Kitchen = estate.Kitchen;
            response.Bathroom = estate.Bathroom;
            response.FloorCount = estate.FloorCount;
            response.Garage = estate.Garage;
            response.Elevator = estate.Elevator;
            response.Terrace = estate.Terace;
            response.PropertySquareFeet = estate.PropertySquareFeet;
            response.GarageSquareFeet = estate.GarageSquareFeet ?? 0;
            response.GardenSquareFeet = estate.GardenSquareFeet ?? 0;
            response.TerraceSquareFeet = estate.TerraceSquareFeet ?? 0;
            response.Basement = response.Basement;
            response.Comfort = response.Comfort;
            response.Advertiser = response.Advertiser;
            response.AdvertiserId = response.AdvertiserId;
            response.Images = repo.GetGenericImages()
                                        .Where(image => image.EstateId == estate.Id)
                                        .Select(image => image.Id)
                                        .ToList();

            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]   
        //  Adott ingatlan fűtés-részletek.
        //
        public string HeatingDetail(int estateId)
        {
            var response = new HeatingSystemViewModel();
            HeatingSystem heating = repo.GetHeatingSystem(estateId);

            response.ByWood = heating.ByWood;
            response.ByRemote = heating.ByRemote;
            response.ByGas = heating.ByGas;
            response.ByElectricity = heating.ByElectricity;
            response.FloorHeating = heating.FloorHeating;

            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]   
        //  Adott ingatlan villamosság-részletek.
        //
        public string ElectricityDetail(int estateId)
        {
            var response = new ElectricityViewModel();
            Electricity electricity = repo.GetElectricity(estateId);

            response.SunCollector = electricity.SunCollector;
            response.Thermal = electricity.Thermal;
            response.Networked = electricity.Networked;

            return JsonConvert.SerializeObject(response);
        }

        [HttpGet]  
        //  Adott ingatlan szolgáltatás-részletek.
        //
        public string PublicServiceDetail(int estateId)
        {
            var response = new PublicServiceViewModel();
            PublicService publicService = repo.GetPublicService(estateId);

            response.Grocery = publicService.Grocery;
            response.GasStation = publicService.GasStation;
            response.Transport = publicService.Transport;
            response.DrugStore = publicService.DrugStore;
            response.School = publicService.School;
            response.MailDepot = publicService.MailDepot;
            response.MailDepot = publicService.Bank;

            return JsonConvert.SerializeObject(response);
        }

        #endregion
        #region WebAPI POST

        [HttpPost]
        // Új hirdetés.
        //
        public string Upload()
        {
            var response = new ItemPostedResultModel();

            var uploadable = new UploadWrapperModel();
            string data = null;
            var  body = this.HttpContext.Request.Headers.ToArray();
            int length = (int) this.HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferReaderTask = this.HttpContext.Request.Body.ReadAsync(buffer, 0, length);

            if (bufferReaderTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                uploadable = (UploadWrapperModel) JsonConvert.DeserializeObject(data, typeof(UploadWrapperModel), new JsonSerializerSettings());
            }

            System.Func<object, bool> addEstateAdvertisementAsync = delegate(object toAdd) 
            {
                var wrapper = (UploadWrapperModel) toAdd;

                int estateId = repo.AddEstate(wrapper.Estate);

                if (estateId != 0)
                {
                    repo.AddAddress(wrapper.Address, estateId);
                    repo.AddElectricity(wrapper.Electricity, estateId);
                    repo.AddHeatingSystem(wrapper.Heating, estateId);
                    repo.AddPublicService(wrapper.PublicService, estateId);
                    repo.AddWaterSystem(wrapper.Water, estateId);
                    repo.AddAdvertisement(wrapper.Advertisement, estateId, wrapper.Estate.AdvertiserId);
                }

                response.ItemId = estateId.ToString();
                response.Success = true;

                return true;
            };

            var uploadTask = new Task<bool>(addEstateAdvertisementAsync, uploadable);
            uploadTask.Start();
            uploadTask.Wait();

            if (!uploadTask.IsCompletedSuccessfully)
                response = new ItemPostedResultModel();
            
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Hirdetés módosítás.
        //
        public string Modify(int estateId)
        {
            var response = new ItemPostedResultModel();

            var modifiable = new UploadWrapperModel();
            string data = null;
            var  body = this.HttpContext.Request.Headers.ToArray();
            int length = (int) this.HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferReaderTask = this.HttpContext.Request.Body.ReadAsync(buffer, 0, length);

            if (bufferReaderTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                modifiable = (UploadWrapperModel) JsonConvert.DeserializeObject(data, typeof(UploadWrapperModel), new JsonSerializerSettings());
            }

            System.Func<object, bool> modifyEstateAdvertisementAsync = delegate(object toModify) 
            {
                var wrapper = (UploadWrapperModel) toModify;

                repo.UpdateEstate(wrapper.Estate);
                repo.UpdateAddress(wrapper.Address);
                repo.UpdateElectricity(wrapper.Electricity);
                repo.UpdateHeatingSystem(wrapper.Heating);
                repo.UpdatePublicService(wrapper.PublicService);
                repo.UpdateWaterSystem(wrapper.Water);
                repo.UpdateAdvertisement(wrapper.Advertisement);


                response.ItemId = wrapper.Estate.Id.ToString();
                response.Success = true;

                return true;
            };

            var modifyTask = new Task<bool>(modifyEstateAdvertisementAsync, modifiable);
            modifyTask.Start();
            modifyTask.Wait();

            if (!modifyTask.IsCompletedSuccessfully)
                response = new ItemPostedResultModel();

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Hirdetés törlés.
        //
        public string Delete(int estateId)
        {
            var response = new ItemPostedResultModel();

            response.Success = repo.DeleteEstate(estateId);
            response.ItemId = estateId.ToString(); 

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
