using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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
                GenericImage thumbnail = repo.GetEstateThumbnail(estate.Id);

                if (ad == null || address == null)
                    continue;

                if (thumbnail == null) 
                {
                    thumbnail = new GenericImage();
                    thumbnail.Id = "no-image";
                    thumbnail.Extension = ".jpg";
                }

                estateOverview.EstateId = estate.Id;
                estateOverview.Title = ad.Title;
                estateOverview.Price = estate.Price;
                estateOverview.OfferType = ad.OfferType;
                estateOverview.City = address.City;
                estateOverview.PostCode = address.PostCode;
                estateOverview.Street = address.Street;
                estateOverview.Country = address.Country;
                estateOverview.Image = thumbnail.Id + thumbnail.Extension;
                estateOverview.TimePosted = ad.TimePosted;

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
            Electricity electricity = repo.GetElectricity(estateId);
            HeatingSystem heating = repo.GetHeatingSystem(estateId);
            Address address = repo.GetAddress(estateId);
            WaterSystem water = repo.GetWaterSystem(estateId);
            PublicService services = repo.GetPublicService(estateId);
            Advertisement advertisement = repo.GetAdvertisement(estateId);
            List<GenericImage> images = repo.GetEstateImages(estateId);

            response.Estate = estate;
            response.Electricity = electricity;
            response.Heating = heating;
            response.Address = address;
            response.Water = water;
            response.Services = services;
            response.Advertisement = advertisement;
            response.Images = images;

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

        [HttpGet]
        // Véletlenszerű képek.
        //
        public string RandomShowCase()
        {
            List<string> response = null;
            response = repo.GetRandomImages(3);

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

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = "Bejelentkezés szükséges.";
                goto cancelUpload;
            }

            var uploadable = new UploadWrapperModel();
            string data = null;

            int length = (int) this.HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            
            var bufferReaderTask = this.HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferReaderTask.Wait();

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
            
            cancelUpload:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Hirdetés módosítás.
        //
        public string Modify()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = "Bejelentkezés szükséges.";
                goto cancelModify;
            }

            var model = new UploadWrapperModel();
            
            string data = null;            
            int length = (int) this.HttpContext.Request.ContentLength;
            byte[] buffer = new byte[length];
            var bufferReaderTask = this.HttpContext.Request.Body.ReadAsync(buffer, 0, length);
            bufferReaderTask.Wait();

            if (bufferReaderTask.IsCompletedSuccessfully)
            {
                data = System.Text.Encoding.Default.GetString(buffer);
                model = (UploadWrapperModel) JsonConvert.DeserializeObject(data, typeof(UploadWrapperModel));

                repo.UpdateEstate(model.Estate);
                repo.UpdateAddress(model.Address);
                repo.UpdateElectricity(model.Electricity);
                repo.UpdateHeatingSystem(model.Heating);
                repo.UpdatePublicService(model.PublicService);
                repo.UpdateWaterSystem(model.Water);
                repo.UpdateAdvertisement(model.Advertisement);

                response.Message = "A hirdetés sikeresen módosítva lett.";
                response.Success = true;
                response.ItemId = model.Estate.Id.ToString();
            }
            else {
                response.Message = "Hiba történt a hirdetés módosítása közben.";
            }

            cancelModify:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Hirdetés törlés.
        //
        public string Delete(int estateId)
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User)) {
                response.Message = "Bejelentkezés szükséges.";
                goto cancelDelete;
            }

            response.Success = repo.DeleteEstate(estateId);
            response.ItemId = estateId.ToString(); 

            cancelDelete:
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        // Képfeltöltés
        //
        public string ImageUpload()
        {
            var response = new ItemPostedResultModel();

            var images = HttpContext.Request.Form.Files;

            long size = images.Sum(f => f.Length);
            foreach (IFormFile image in images)
            {
                if (image.Length > 0)
                {
                    string cd = Directory.GetCurrentDirectory();
                    string id = System.Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(image.FileName);
                    string path = Path.Combine(cd, "wwwroot", "static", id);
                    path += extension;

                    var imageData = new GenericImage();
                    string json = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString())["imageData"];
                    imageData = (GenericImage) JsonConvert.DeserializeObject(json, typeof(GenericImage));

                    using (FileStream stream = System.IO.File.Create(path))
                    {
                        var copyImageToStreamTask = image.CopyToAsync(stream);
                        copyImageToStreamTask.Wait();

                        if (copyImageToStreamTask.IsCompletedSuccessfully)
                        {
                            imageData.Id = id;
                            imageData.StorageSize = (int) image.Length;
                            
                            imageData.UserId = imageData.UserId;
                            imageData.EstateId = imageData.EstateId;
                            imageData.Category = imageData.Category;
                            imageData.Title = imageData.Title;
                            imageData.DescriptionDetail = imageData.DescriptionDetail;
                            imageData.Extension = extension;

                            bool storeSuccess = repo.AddImage(imageData);

                            if (storeSuccess) {
                                response.ItemId = imageData.Id;
                                response.Success = true;
                                response.Message = "A képfeltöltés sikeresen megtörtént.";
                            }
                        }                        
                        else {
                            response.Message = "Sikertelen képfeltöltés.";
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
