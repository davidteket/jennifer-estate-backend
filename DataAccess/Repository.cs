using backend.DataAccess.Entities;
using backend.DataAccess.Entities.Identity;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace backend.DataAccess
{
    public class Repository : IRepository
    {
        private JenniferEstateContext ctx;
        public Repository()
        {
            ctx = new JenniferEstateContext(Startup.ctxOptionsBuilder.Options);
        }

        public List<Address> GetAddresses() => ctx.Address.ToList();
        public Address GetAddress(int estateId) 
        {
            Address result = null;

            var address = ctx.Address.Select(addr => addr)
                             .Where(addr => addr.EstateId == estateId);
            if (address.Count() > 0)
                result = address.First();

            return result; 
        }

        public List<Advertisement> GetAdvertisements() => ctx.Advertisement.ToList();
        public Advertisement GetAdvertisement(int estateId)
        {
            Advertisement result = null;

            var advert = ctx.Advertisement.Select(ad => ad)
                             .Where(ad => ad.EstateId == estateId);
            if (advert.Count() > 0)
                result = advert.First();

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
                result = electricity.First();

            return result;                
        }
        public WaterSystem GetWaterSystem(int estateId)
        {
            WaterSystem result = null;

            var water = ctx.WaterSystem.Select(w => w)
                             .Where(w => w.EstateId == estateId);
                             
            if (water.Count() > 0)
                result = water.First();

            return result;                
        }

        public PublicService GetPublicService(int estateId) 
        {
            PublicService result = null;

            var service = ctx.PublicService.Select(s => s)
                             .Where(s => s.EstateId == estateId);
                             
            if (service.Count() > 0)
                result = service.First();

            return result;                
        }

        public Estate GetEstate(int estateId)
        {
            Estate result = null;

            var estate = ctx.Estate.Select(e => e)
                             .Where(e => e.Id == estateId);
                             
            if (estate.Count() > 0)
                result = estate.First();

            return result;                
        }

        public List<Estate> GetEstates(int from, int limit)
        {
            List<Estate> result = ctx.Estate.Select(e => e)
                                            .Where(e => e.Id >= from)
                                            .Take(limit).ToList();

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

            var estates = GetEstatesSimpleCriteria(estateType, minPrice, maxPrice, city).Select(s => s)
                     .Where(e => (e.SquareFeet >= minSquareFeet && e.SquareFeet <= maxSquareFeet) ||
                                 (e.BuiltAt >= minBuiltAt && e.BuiltAt <= maxBuiltAt) ||
                                 (e.Grade.ToUpper() == grade.ToUpper()) ||
                                 (e.Room == roomCount) ||
                                 (e.Kitchen == kitchenCount) ||
                                 (e.Bathroom == bathroomCount) ||
                                 (e.FloorCount == floors) ||
                                 ((e.RefurbishedAt == null ? false : true) == refurbished) ||
                                 (e.Garage == (garage ? 1 : 0)) ||
                                 (e.Elevator == (elevator ? 1 : 0)) ||
                                 (e.Garden == (garden ? 1 : 0)) ||
                                 (e.Terace == (terace ? 1 : 0)) || 
                                 (e.Basement == (basement ? 1 : 0))
                     ).ToList();

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
        public string GetProfilePictureId(string employeeId) => ctx.GenericImage.Select(image => image)
                                                                          .Where(image => image.UserId == employeeId)
                                                                          .First()
                                                                          .Id;
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
                                                                         .First();
        public List<User> GetUsers()
        {
            var result = new List<User>();

            return result;
        }
        public List<Popularity> GetPopularities() => ctx.Popularity.ToList();
        public List<WaterSystem> GetWaterSystems() => ctx.WaterSystem.ToList();

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

        public CompanyDetails GetCompanyDetails() => ctx.CompanyDetails.First();

        public bool CheckInvitationStatus(Invitation invitation)
        {
            bool success = false;

            success = ctx.Invitation.Select(i => i)
                                    .Any(i => (i.Id == invitation.Id) && (i.Expired == 0));

            return success;
        }

        public void InvalidateInvitationTicket(string invitationId)
        {
            Invitation invitation = ctx.Invitation.Select(i => i).Where(i => i.Id == invitationId).First();
            invitation.Expired = 1;

            ctx.SaveChanges();
        }

        public void UpdateEstate(Estate estate)
        {
            var toUpdate = ctx.Estate.Select(e => e)
                                     .Where(e => e.Id == estate.Id)
                                     .First();
                                     
            toUpdate.SquareFeet = estate.SquareFeet;
            toUpdate.Category = estate.Category;
            toUpdate.BuiltAt = estate.BuiltAt;
            toUpdate.RefurbishedAt = estate.RefurbishedAt;
            toUpdate.Grade = estate.Grade;
            toUpdate.Room = estate.Room;
            toUpdate.Kitchen = estate.Kitchen;
            toUpdate.Bathroom = estate.Bathroom;
            toUpdate.FloorCount = estate.FloorCount;
            toUpdate.Garage = estate.Garage;
            toUpdate.Elevator = estate.Elevator;
            toUpdate.Terace = estate.Terace;
            toUpdate.PropertySquareFeet = estate.PropertySquareFeet;
            toUpdate.GarageSquareFeet = estate.GarageSquareFeet ?? 0;
            toUpdate.GardenSquareFeet = estate.GardenSquareFeet ?? 0;
            toUpdate.TerraceSquareFeet = estate.TerraceSquareFeet ?? 0;
            toUpdate.Basement = estate.Basement;
            toUpdate.Comfort = estate.Comfort;
            toUpdate.AdvertiserId = estate.AdvertiserId;
            toUpdate.Garden = estate.Garden;
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
            toUpdate.Thermal = electricity.Thermal;
        }
        
        public void UpdateHeatingSystem(HeatingSystem heating)
        {
            var toUpdate = ctx.HeatingSystem.Select(h => h)
                                            .Where(h => h.EstateId == heating.EstateId)
                                            .First();

            toUpdate.FloorHeating = heating.FloorHeating;
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

            toUpdate.Bank = publicService.Bank;
            toUpdate.DrugStore = publicService.DrugStore;
            toUpdate.GasStation = publicService.GasStation;
            toUpdate.Grocery = publicService.Grocery;
            toUpdate.MailDepot = publicService.MailDepot;
            toUpdate.Transport = publicService.Transport;
            toUpdate.School = publicService.School;

            ctx.SaveChanges();
        }
        
        public void UpdateWaterSystem(WaterSystem waterSystem)
        {
            var toUpdate = ctx.WaterSystem.Select(w => w)
                                          .Where(w => w.EstateId == waterSystem.EstateId)
                                          .First();

            toUpdate.AvailabilityType = waterSystem.AvailabilityType;

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

        public void AddWaterSystem(WaterSystem waterSystem, int estateId)
        {
            waterSystem.EstateId = estateId;
            ctx.WaterSystem.Add(waterSystem);
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

            var advertisement = ctx.Advertisement.Select(a => a)
                                    .Where(a => a.EstateId == estateId)
                                    .First();

            ctx.Advertisement.Remove(advertisement);

            var waterSystem = ctx.WaterSystem.Select(w => w)
                                    .Where(w => w.EstateId == estateId)
                                    .First();

            var publicService = ctx.PublicService.Select(p => p)
                                    .Where(p => p.EstateId == estateId)
                                    .First();

            var heatingSystem = ctx.HeatingSystem.Select(h => h)
                                    .Where(h => h.EstateId == estateId)
                                    .First();

            var electricity = ctx.Electricity.Select(e => e)
                                    .Where(e => e.EstateId == estateId)
                                    .First();

            var address = ctx.Address.Select(a => a)
                                    .Where(a => a.EstateId == estateId)
                                    .First();

            var images = ctx.GenericImage.Select(gi => gi)
                                         .Where(gi => gi.EstateId == estateId)
                                         .ToList();

            var estate = ctx.Estate.Select(e => e)
                                    .Where(e => e.Id == estateId)
                                    .First();

            ctx.WaterSystem.Remove(waterSystem);                                    
            ctx.SaveChanges();
            ctx.PublicService.Remove(publicService);
            ctx.SaveChanges();
            ctx.HeatingSystem.Remove(heatingSystem);
            ctx.SaveChanges();
            ctx.Electricity.Remove(electricity);
            ctx.SaveChanges();
            ctx.Address.Remove(address);
            ctx.SaveChanges();
            ctx.GenericImage.RemoveRange(images);
            ctx.SaveChanges();
            ctx.Estate.Remove(estate);
            int affected = ctx.SaveChanges();

            if (affected == 1)
                success = true;

            string cd = Directory.GetCurrentDirectory();
            foreach(GenericImage image in images)
            {
                string name = image.Id;
                string extension = image.Extension;
                string path = Path.Combine(cd, "static", name);
                path += extension;

                File.Delete(path);
            }

            return success;
        }

        public void AddInvitation(string inviteeId) 
        {
            var inv = new Invitation();
            ctx.Invitation.Add(inv);
            ctx.SaveChanges();
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
        }

        public bool DeleteEmployee(string employeeId)
        {
            bool success = false;

            // Need to assign the related advertisements among the remaining employees.
            //

            // TODO
            

            return success;
        }
    }
}
