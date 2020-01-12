using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using DunakanyarHouseIngatlan.DataAccess.Entities;
using DunakanyarHouseIngatlan.Models;
using DunakanyarHouseIngatlan.DataAccess.Entities.Identity;
using DunakanyarHouseIngatlan.DataAccess;
using DunakanyarHouseIngatlan.Services;

namespace DunakanyarHouseIngatlan.Controllers
{
    public class EstateController : Controller
    {
        private IRepository _repo;
        private Serializer _serializer;
        private SignInManager<User> _signInManager;

        public EstateController(IRepository repo, SignInManager<User> signInManager)
        {
            _serializer = new Serializer();
            _repo = repo;
            _signInManager = signInManager;
        }


        #region WebAPI GET  
 
        // Hirdetések darabszámának lekérdezése.
        //
        [HttpGet]
        public string Count()
        {
            var response = new ItemPostedResultModel();

            int count = _repo.CountEstates();

            if (!(count >= 0))
                response.Message = "Hiba történt az ingatlanok darabszámának lekérdezésekor.";
            else {
                response.ItemId = count.ToString();
                response.Message = "Az elérhető ingatlanok darabszáma sikeresen lekérve.";
                response.Success = true;

            }

            return JsonConvert.SerializeObject(response);
        }

        //  Adott darabszámú ingatlan lekérés.
        //
        [HttpGet]
        public string Load(int? fromId = null, int limit = 20)
        {
            var response = new List<EstateOverviewViewModel>();

            List<Estate> estates = _repo.GetEstates(fromId, limit);
            foreach (Estate estate in estates)
            {
                var estateOverview = new EstateOverviewViewModel();

                Advertisement ad = _repo.GetAdvertisement(estate.Id);
                Address address = _repo.GetAddress(estate.Id);
                GenericImage thumbnail = _repo.GetEstateThumbnail(estate.Id);

                if (ad == null || address == null)
                    continue;

                if (thumbnail == null) 
                {
                    thumbnail = new GenericImage();
                    thumbnail.Id = "default/no-image";
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
  
        //  Adott ingatlan lekérés.
        //
        [HttpGet] 
        public string Detail(int? estateId = null)
        {
            var response = new EstateDetailsViewModel();

            Estate estate = null;
            int id = -1;

            if (estateId != null) {
                id = (int) estateId;
                estate = _repo.GetEstate(id);
            }

            if (estate == null)
                response = null;
            else 
            {
                Electricity electricity = _repo.GetElectricity(id);
                HeatingSystem heating = _repo.GetHeatingSystem(id);
                Address address = _repo.GetAddress(id);
                PublicService services = _repo.GetPublicService(id);
                Advertisement advertisement = _repo.GetAdvertisement(id);
                List<GenericImage> images = _repo.GetEstateImages(id);

                response.Estate = estate;
                response.Electricity = electricity;
                response.Heating = heating;
                response.Address = address;
                response.Services = services;
                response.Advertisement = advertisement;
                response.Images = images;
            }

            return JsonConvert.SerializeObject(response);
        }
  
        //  Adott ingatlan fűtés-részletek.
        //
        [HttpGet] 
        public string HeatingDetail(int estateId)
        {
            var response = new HeatingSystemViewModel();
            HeatingSystem heating = _repo.GetHeatingSystem(estateId);

            response.ByWood = heating.ByWood;
            response.ByRemote = heating.ByRemote;
            response.ByGas = heating.ByGas;
            response.ByElectricity = heating.ByElectricity;
            response.ByFloorHeating = heating.ByFloorHeating;
            response.ByCombined = heating.ByCombined;
            response.ByGasConvector = heating.ByGasConvector;
            response.ByNetworked = heating.ByNetworked;
            response.ByChimney = heating.ByChimney;
            response.ByCirculation = heating.ByCirculation;
            response.ByCockle = heating.ByCockle;

            return JsonConvert.SerializeObject(response);
        }
  
        //  Adott ingatlan villamosság-részletek.
        //
        [HttpGet] 
        public string ElectricityDetail(int estateId)
        {
            var response = new ElectricityViewModel();
            Electricity electricity = _repo.GetElectricity(estateId);

            response.SunCollector = electricity.SunCollector;
            response.PowerWall = electricity.PowerWall;
            response.Networked = electricity.Networked;

            return JsonConvert.SerializeObject(response);
        }
 
        //  Adott ingatlan szolgáltatás-részletek.
        //
        [HttpGet] 
        public string PublicServiceDetail(int estateId)
        {
            var response = new PublicServiceViewModel();
            PublicService publicService = _repo.GetPublicService(estateId);

            response.HasGroceryNearby = publicService.HasGroceryNearby;
            response.HasGasStationNearby = publicService.HasGasStationNearby;
            response.HasTransportNearby = publicService.HasTransportNearby;
            response.HasDrugStoreNearby = publicService.HasDrugStoreNearby;
            response.HasSchoolNearby = publicService.HasSchoolNearby;
            response.HasMailDepotNearby = publicService.HasMailDepotNearby;
            response.HasMailDepotNearby = publicService.HasBankNearby;
            response.HasEntertainmentServicesNearby = publicService.HasEntertainmentServicesNearby;

            return JsonConvert.SerializeObject(response);
        }

        // Véletlenszerű képek.
        //
        [HttpGet]
        public string RandomShowCase()
        {
            List<string> response = null;
            response = _repo.GetRandomImages(3);

            return JsonConvert.SerializeObject(response);
        }

        // Keresés.
        //
        [HttpGet]
        public string Search()
        {
            var response = new List<EstateOverviewViewModel>();

            


            return JsonConvert.SerializeObject(response);
        }

        #endregion
        #region WebAPI POST

        // Új hirdetés.
        //
        [HttpPost]
        public string Upload()
        {
            var response = new ItemPostedResultModel();

            if (User == null || !_signInManager.IsSignedIn(User)) 
            {
                response.ItemId = null;
                response.Message = "Bejelentkezés szükséges.";
                response.Success = false;

                System.Console.WriteLine(response.Message);
            }
            else 
            {
                var uploadable = new UploadWrapperModel();
                string data = _serializer.GetRequestContent(this.HttpContext);

                if (data != null)
                    uploadable = (UploadWrapperModel) JsonConvert.DeserializeObject(data, typeof(UploadWrapperModel), new JsonSerializerSettings());

                System.Func<object, bool> addEstateAdvertisementAsync = delegate(object toAdd) 
                {
                    var wrapper = (UploadWrapperModel) toAdd;

                    int estateId = _repo.AddEstate(wrapper.Estate);

                    if (estateId != 0)
                    {
                        _repo.AddAddress(wrapper.Address, estateId);
                        _repo.AddElectricity(wrapper.Electricity, estateId);
                        _repo.AddHeatingSystem(wrapper.Heating, estateId);
                        _repo.AddPublicService(wrapper.PublicService, estateId);
                        _repo.AddAdvertisement(wrapper.Advertisement, estateId, wrapper.Estate.AdvertiserId);
                    }

                    response.ItemId = estateId.ToString();
                    response.Message = "Sikeres hirdetés feltöltés.";
                    response.Success = true;

                    System.Console.WriteLine(response.Message);
                    return true;
                };

                var uploadTask = new Task<bool>(addEstateAdvertisementAsync, uploadable);
                uploadTask.Start();
                uploadTask.Wait();

                if (!uploadTask.IsCompletedSuccessfully)
                    response.Success = true;
            }
            
            return JsonConvert.SerializeObject(response);
        }

        // Hirdetés módosítás.
        //
        [HttpPost]
        public string Modify()
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = "Bejelentkezés szükséges.";

            var model = new UploadWrapperModel();            
            string data = _serializer.GetRequestContent(this.HttpContext);

            if (data!= null)
            {
                model = (UploadWrapperModel) JsonConvert.DeserializeObject(data, typeof(UploadWrapperModel));

                _repo.UpdateEstate(model.Estate);
                _repo.UpdateAddress(model.Address);
                _repo.UpdateElectricity(model.Electricity);
                _repo.UpdateHeatingSystem(model.Heating);
                _repo.UpdatePublicService(model.PublicService);
                _repo.UpdateAdvertisement(model.Advertisement);

                response.ItemId = model.Estate.Id.ToString();
                response.Message = "A hirdetés sikeresen módosítva lett.";
                response.Success = true;
            }
            else
                response.Message = "Hiba történt a hirdetés módosításakor.";
                
            return JsonConvert.SerializeObject(response);
        }

        // Hirdetés törlés.
        //
        [HttpPost]
        public string Delete(int estateId)
        {
            var response = new ItemPostedResultModel();

            if (!Startup.SignInManager.IsSignedIn(User))
                response.Message = "Bejelentkezés szükséges.";

            bool result = _repo.DeleteEstate(estateId);
            if (result == true) 
            {
                response.ItemId = estateId.ToString();
                response.Message = "A hirdetés törlése sikeresen megtörtént.";
                response.Success = true;
            }
            else
                response.Message = "Hiba történt a hirdetés törlésekor.";
            
            return JsonConvert.SerializeObject(response);
        }

        // Képfeltöltés
        //
        [HttpPost]
        public string ImageUpload()
        {
            var response = new ItemPostedResultModel();

            var images = HttpContext.Request.Form.Files;
            long size = images.Sum(f => f.Length);
            int queued = HttpContext.Request.Form.Files.Count();
            string[] messages = new string[queued];
            int queueNumber = 0;

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

                            bool storeSuccess = _repo.AddImage(imageData);

                            if (storeSuccess) 
                            {
                                response.ItemId = imageData.Id;
                                messages[queueNumber] = image.FileName;
                                response.Success = true;
                                ++queueNumber;
                            }
                        }                        
                        else {
                            response.Message = "Egy vagy több kép feltöltése nem sikerült.";
                            response.Success = false;
                            break;
                        }
                    }
                }
            }

            if (response.Success == true)
                response.Message = JsonConvert.SerializeObject(messages);

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
