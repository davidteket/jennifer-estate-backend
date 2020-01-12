using DunakanyarHouseIngatlan.DataAccess.Entities;
using DunakanyarHouseIngatlan.DataAccess.Entities.Identity;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace DunakanyarHouseIngatlan.DataAccess
{
    public class Repository : IRepository
    {
        private JenniferEstateContext ctx;
        
        public Repository()
        {
            ctx = new JenniferEstateContext(Startup.ctxOptionsBuilder.Options);
        }

        public int CountEstates()
        {
            int result = -1;

            result = ctx.Estate.Count();

            return result;
        }

        public List<Address> GetAddresses() => ctx.Address.ToList();
        public Address GetAddress(int estateId) 
        {
            Address result = null;

            var address = ctx.Address.Select(addr => addr)
                             .Where(addr => addr.EstateId == estateId);
            if (address.Count() > 0)
                result = address.FirstOrDefault();

            return result; 
        }

        public List<Advertisement> GetAdvertisements() => ctx.Advertisement.ToList();
        public Advertisement GetAdvertisement(int estateId)
        {
            Advertisement result = null;

            var advert = ctx.Advertisement.Select(ad => ad)
                             .Where(ad => ad.EstateId == estateId);
            if (advert.Count() > 0)
                result = advert.FirstOrDefault();

            return result;                
        }
        public List<ClientRequest> GetClients() => ctx.ClientRequest.ToList();
        public List<Electricity> GetElectricities() => ctx.Electricity.ToList();
        public Electricity GetElectricity(int estateId) 
        {
            Electricity result = null;

            var electricity = ctx.Electricity.Select(e => e)
                             .Where(e => e.EstateId == estateId);

            if (electricity.Count() > 0)
                result = electricity.FirstOrDefault();

            return result;                
        }

        public PublicService GetPublicService(int estateId) 
        {
            PublicService result = null;

            var service = ctx.PublicService.Select(s => s)
                             .Where(s => s.EstateId == estateId);
                             
            if (service.Count() > 0)
                result = service.FirstOrDefault();

            return result;                
        }

        // Adott id-jú ingatlan  lekérdezése.
        //
        public Estate GetEstate(int estateId)
        {
            Estate result = null;

            var estate = ctx.Estate.Select(e => e)
                             .Where(e => e.Id == estateId);
                             
            if (estate.Count() > 0)
                result = estate.FirstOrDefault();

            return result;                
        }

        // Adott Id-tól felmenőleg lekérdezi az ingatlanokat. 
        // Legfeljebb 50-et lehet lekérdezni egyszerre.
        //
        public List<Estate> GetEstates(int? fromId, int limit)
        {
            List<Estate> result = new List<Estate>();
            int max = 20;

            if (limit > max || limit <= 0)
                limit = max;
            if (fromId == null) {
                result = ctx.Estate.Select(e => e).Take(limit).ToList();
            }
            else {
                Estate item = ctx.Estate.Where(e => e.Id == fromId).FirstOrDefault();
                if (item == null)
                    result = ctx.Estate.Select(e => e).Take(limit).ToList();
                else {
                    bool found = false;
                    int took = 0;
                    foreach (Estate entry in ctx.Estate) {
                        if (entry.Id == fromId) {
                            found = true;
                        }
                        if (found && took < limit) {
                            result.Add(entry);
                            ++took;
                        }
                    }
                }
            }

            return result;    
        }

        public List<Estate> GetEstatesSimpleCriteria(string estateType, int minPrice, int maxPrice, string city)
        {
            var result = new List<Estate>();

            List<Estate> estates = ctx.Estate.Select(e => e).Where(e => (e.Category.ToUpper() == estateType.ToUpper()) ||
                                                                       (e.Price >= minPrice && e.Price <= maxPrice))
                                                           .ToList();

            bool contains = ctx.Address.Any(c => c.City.ToUpper() == city.ToUpper());
            if ((estates.Count > 0) && contains) {
                foreach (Estate estate in estates) 
                {
                    Address address = ctx.Address.Select(a => a).Where(a => a.EstateId == estate.Id).First();
                    if (address.City.ToUpper() == city.ToUpper())
                        result.Add(estate);
                }
            }
            else {
                result = estates;
            }

            return result;
        }

        public List<Estate> GetEstatesComplexCriteria(string estateType, int minPrice, int maxPrice, string city,

                                                      int minSquareFeet, int maxSquareFeet, System.DateTime minBuiltAt, System.DateTime maxBuiltAt,
                                                      string grade, /* elhelyezkedés */ int roomCount, int kitchenCount, int bathroomCount,
                                                      int floors, bool refurbished, bool garage, bool elevator, bool garden, bool terace,
                                                      bool basement, 
                                                      
                                                      bool suncollector, bool networkedHeating, bool woodenHeating, /* */
                                                      /* */ bool remoteHeating, bool gasHeating, bool electricityHeating, bool floorHeating,
                                                      bool grocery, bool gasStation, bool publicTransport, bool drugStore, bool school, 
                                                      bool mailDepot, bool bank) {

            var result = new List<Estate>();

           
            // TODO


            return result;
        }

        public List<EstateClient> GetEstateClients() => ctx.EstateClient.ToList();
        public List<GenericImage> GetEstateImages(int estateId)
        {
            List<GenericImage> result = ctx.GenericImage.Select(i => i)
                                                        .Where(i => i.EstateId == estateId)
                                                        .ToList();

            return result;
        }
        public GenericImage GetProfilePictureId(string employeeId) => ctx.GenericImage.Select(image => image)
                                                                          .Where(image => image.UserId == employeeId)
                                                                          .FirstOrDefault();
        public GenericImage GetEstateThumbnail(int estateId)
        {
            GenericImage result = null;

            var image = ctx.GenericImage.Select(i => i)
                             .Where(i => i.EstateId == estateId);
                             
            if (image.Count() > 0)
                result = image.First();

            return result;                
        }
        public List<HeatingSystem> GetHeatingSystems() => ctx.HeatingSystem.ToList();
        public HeatingSystem GetHeatingSystem(int estateId) => ctx.HeatingSystem.Select(hsys => hsys)
                                                                         .Where(hsys => hsys.EstateId == estateId)
                                                                         .FirstOrDefault();
        public List<User> GetUsers()
        {
            var result = new List<User>();

            return result;
        }
        public List<Popularity> GetPopularities() => ctx.Popularity.ToList();
        

        public byte[] GetImage(int imageId) 
        {
            byte[] result = null;
            string path = "/home/dt/Desktop/project/application/database/images/" + imageId.ToString() + ".tga";

            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileInfo = new FileInfo(path);
            int size = (int) fileInfo.Length;
            result = new byte[size];
            stream.ReadAsync(result, 0, size);

            return result;
        }

        public List<string> GetRandomImages(int count)
        {
            var result = new List<string>();

            var generator = new System.Random();
            var images = ctx.GenericImage.Select(i => i)
                                        .Where(i => i.EstateId != null)
                                        .ToList();
            int length = images.Count();
            if (length < 3) {
                for (int i = 0; i < 3; ++i) 
                    result.Add("default/no-image.jpg");

                return result;
            }

            for (int i = 0; i < count; ++i) 
            {
                int j = generator.Next(0, length - 1);
                GenericImage image = images.ElementAt(j);

                string name = image.Id;
                name += image.Extension;

                if (result.Contains(name))
                {
                    ++count;
                    continue;
                }

                result.Add(name);
            }

            return result;
        }

        public void UpdateEstate(Estate estate)
        {
            var toUpdate = ctx.Estate.Select(e => e)
                                     .Where(e => e.Id == estate.Id)
                                     .First();
                                     
            toUpdate.Area = estate.Area;
            toUpdate.HasDisabledFriendly = estate.HasDisabledFriendly;
            toUpdate.HasHeatIsolated = estate.HasHeatIsolated;
            toUpdate.HasInnerHeightGreatherThan3Meters = estate.HasInnerHeightGreatherThan3Meters;
            toUpdate.HasParticipatedInThePanelProgram = estate.HasParticipatedInThePanelProgram;
            toUpdate.HasSeparateWcAndBathroom = estate.HasSeparateWcAndBathroom;
            toUpdate.Outlook = estate.Outlook;
            toUpdate.Roof = estate.Roof;                                     
            toUpdate.TotalSquareFeet = estate.TotalSquareFeet;
            toUpdate.LandSquareFeet = estate.LandSquareFeet;
            toUpdate.Category = estate.Category;
            toUpdate.BuiltAt = estate.BuiltAt;
            toUpdate.RefurbishedAt = estate.RefurbishedAt;
            toUpdate.Quality = estate.Quality;
            toUpdate.RoomCount = estate.RoomCount;
            toUpdate.KitchenCount = estate.KitchenCount;
            toUpdate.BathroomCount = estate.BathroomCount;
            toUpdate.FloorCount = estate.FloorCount;
            toUpdate.GarageSquareFeet = estate.GarageSquareFeet;
            toUpdate.HasElevator = estate.HasElevator;
            toUpdate.HasTerace = estate.HasTerace;
            toUpdate.GarageSquareFeet = estate.GarageSquareFeet;
            toUpdate.GardenSquareFeet = estate.GardenSquareFeet;
            toUpdate.TerraceSquareFeet = estate.TerraceSquareFeet;
            toUpdate.HasBasement = estate.HasBasement;
            toUpdate.Comfort = estate.Comfort;
            toUpdate.AdvertiserId = estate.AdvertiserId;
            toUpdate.GardenSquareFeet = estate.GardenSquareFeet;
            toUpdate.Price = estate.Price;

            ctx.SaveChanges();
        }
        
        public void UpdateAddress(Address address)
        {
            var toUpdate = ctx.Address.Select(a => a)
                                      .Where(a => a.EstateId == address.EstateId)
                                      .First();

            toUpdate.Country = address.County;
            toUpdate.County = address.County;
            toUpdate.City = address.City;
            toUpdate.Street = address.Street;
            toUpdate.PostCode = address.PostCode;
            toUpdate.HouseNumber = address.HouseNumber;
            toUpdate.FloorNumber = address.FloorNumber;
            toUpdate.Door = address.Door;
            
            ctx.SaveChanges();
        }
        
        public void UpdateElectricity(Electricity electricity)
        {
            var toUpdate = ctx.Electricity.Select(e => e)
                                          .Where(e => e.EstateId == electricity.EstateId)
                                          .First();

            toUpdate.Networked = electricity.Networked;
            toUpdate.SunCollector = electricity.SunCollector;
            toUpdate.PowerWall = electricity.PowerWall;
        }
        
        public void UpdateHeatingSystem(HeatingSystem heating)
        {
            var toUpdate = ctx.HeatingSystem.Select(h => h)
                                            .Where(h => h.EstateId == heating.EstateId)
                                            .First();

            toUpdate.ByFloorHeating = heating.ByFloorHeating;
            toUpdate.ByWood = heating.ByWood;
            toUpdate.ByRemote = heating.ByRemote;
            toUpdate.ByGas = heating.ByGas;
            toUpdate.ByElectricity = heating.ByElectricity;

            ctx.SaveChanges();
        }
        
        public void UpdatePublicService(PublicService publicService)
        {
            var toUpdate = ctx.PublicService.Select(p => p)
                                            .Where(p => p.EstateId == publicService.EstateId)
                                            .First();

            toUpdate.HasBankNearby = publicService.HasBankNearby;
            toUpdate.HasDrugStoreNearby = publicService.HasDrugStoreNearby;
            toUpdate.HasGasStationNearby = publicService.HasGasStationNearby;
            toUpdate.HasGroceryNearby = publicService.HasGroceryNearby;
            toUpdate.HasMailDepotNearby = publicService.HasMailDepotNearby;
            toUpdate.HasTransportNearby = publicService.HasTransportNearby;
            toUpdate.HasSchoolNearby = publicService.HasSchoolNearby;
            toUpdate.HasEntertainmentServicesNearby = publicService.HasEntertainmentServicesNearby;

            ctx.SaveChanges();
        }
        
        public void UpdateAdvertisement(Advertisement advertisement)
        {
            var toUpdate = ctx.Advertisement.Select(a => a)
                                            .Where(a => a.EstateId == advertisement.EstateId)
                                            .First();

            toUpdate.DescriptionDetail = advertisement.DescriptionDetail;
            toUpdate.LastModification = advertisement.LastModification;
            toUpdate.OfferType = advertisement.OfferType;
            toUpdate.OrderOfAppearance = advertisement.OrderOfAppearance;
            toUpdate.Title = advertisement.Title;
            toUpdate.AdvertiserId = advertisement.AdvertiserId;

            ctx.SaveChanges();
        }

        public int AddEstate(Estate estate)
        {
            int estateId = 0;

            ctx.Estate.Add(estate);
            ctx.SaveChanges();

            estateId = ctx.Estate.Select(e => e.Id)
                                     .Max();

            return estateId;
        }

        public void AddAddress(Address address, int estateId)
        {
            address.EstateId = estateId;
            ctx.Address.Add(address);
            ctx.SaveChanges();
        }

        public void AddElectricity(Electricity electricity, int estateId)
        {
            electricity.EstateId = estateId;
            ctx.Electricity.Add(electricity);
            ctx.SaveChanges();
        }

        public void AddHeatingSystem(HeatingSystem heatingSystem, int estateId)
        {
            heatingSystem.EstateId = estateId;
            ctx.HeatingSystem.Add(heatingSystem);
            ctx.SaveChanges();
        }

        public void AddPublicService(PublicService publicService, int estateId)
        {
            publicService.EstateId = estateId;
            ctx.PublicService.Add(publicService);
            ctx.SaveChanges();
        }

        public void AddAdvertisement(Advertisement advertisement, int estateId, string advertiserId)
        {
            advertisement.EstateId = estateId;
            advertisement.AdvertiserId = advertiserId;
            ctx.Advertisement.Add(advertisement);
            ctx.SaveChanges();
        }

        public bool DeleteEstate(int estateId)
        {   
            bool success = false;

            var advertisement = GetAdvertisement(estateId);
            if (advertisement != null) {
                ctx.Advertisement.Remove(advertisement);
                ctx.SaveChanges();
            }

            var publicService = GetPublicService(estateId);
            if (publicService != null) {
                ctx.PublicService.Remove(publicService);
                ctx.SaveChanges();
            }

            var heatingSystem = GetHeatingSystem(estateId);
            if (heatingSystem != null) {
                ctx.HeatingSystem.Remove(heatingSystem);
                ctx.SaveChanges();
            }

            var electricity = GetElectricity(estateId);
            if (electricity != null) {
                ctx.Electricity.Remove(electricity);
                ctx.SaveChanges();
            }

            var address = GetAddress(estateId);
            if (address != null) {
                ctx.Address.Remove(address);
                ctx.SaveChanges();
            }

            var images = GetEstateImages(estateId);
            if (images != null) {
                ctx.GenericImage.RemoveRange(images);
                ctx.SaveChanges();
            }

            var estate = GetEstate(estateId);
            if (estate != null) {
                ctx.Estate.Remove(estate);
                ctx.SaveChanges();
            }

            string cd = Directory.GetCurrentDirectory();
            foreach(GenericImage image in images)
            {
                string name = image.Id;
                string extension = image.Extension;
                string path = Path.Combine(cd, "wwwroot", "static", name);
                path += extension;

                File.Delete(path);
            }

            return success;
        }

        public bool AddImage(GenericImage image)
        {
            bool success = false;

            ctx.GenericImage.Add(image);
            int affected = ctx.SaveChanges();

            if (affected == 1)
                success = true;

            return success;
        }

        public void AddClientRequest(ClientRequest clientRequest)
        {
            ctx.ClientRequest.Add(clientRequest);
            ctx.SaveChanges();
        }

        public bool DeleteEmployee(string employeeId)
        {
            bool success = false;

            // TODO
            

            return success;
        }

        public bool DeleteAllEntries()
        {
            bool success = false;

            List<Estate> estates = ctx.Estate.Select(e => e).ToList();
            foreach (Estate estate in estates)
                this.DeleteEstate(estate.Id);

            List<User> employees = ctx.User.Select(u => u).Where(u => u.UserName.ToUpper() != Startup.DeveloperUser.ToUpper()).ToList();
            foreach (User employee in employees) 
            {
                var deleteUserTask = Startup.UserManager.DeleteAsync(employee);
                deleteUserTask.Wait();

            }

            List<UserRole> roles = ctx.UserRole.Select(r => r).Where(r => r.NormalizedName != Startup.DeveloperRole.ToUpper()).ToList();
            foreach (UserRole role in roles)
            {
                var deleteRoleTask = Startup.RoleManager.DeleteAsync(role);
                deleteRoleTask.Wait();
            }
            

            return success;
        }
    }
}
