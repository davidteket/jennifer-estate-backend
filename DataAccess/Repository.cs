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
        public Address GetAddress(int estateId) => ctx.Address.Select(address => address)
                                                              .Where(address => address.EstateId == estateId)
                                                              .First();
        public List<Advertisement> GetAdvertisements() => ctx.Advertisement.ToList();
        public Advertisement GetAdvertisement(int estateId) => ctx.Advertisement.Select(ad => ad)
                                                                                .Where(ad => ad.EstateId == estateId)
                                                                                .First();
        public List<Client> GetClients() => ctx.Client.ToList();
        public List<Electricity> GetElectricities() => ctx.Electricity.ToList();
        public Electricity GetElectricity(int estateId) => ctx.Electricity.Select(electricity => electricity)
                                                                   .Where(electricity => electricity.EstateId == estateId)
                                                                   .First();
        public PublicService GetPublicService(int estateId) => ctx.PublicService.Select(service => service)
                                                                                .Where(service => service.EstateId == estateId)
                                                                                .First();

        public Estate GetEstate(int id)
        {
            Estate result = ctx.Estate.Select(e => e)
                                      .Where(estate => estate.Id == id)
                                      .First();

            return result;
        }
        public List<Estate> GetEstates(int from, int limit)
        {
            List<Estate> result = ctx.Estate.Select(e => e)
                                            .Where(e => e.Id >= from)
                                            .Take(limit).ToList();

            return result;    
        }

        public List<EstateClient> GetEstateClients() => ctx.EstateClient.ToList();
        public List<GenericImage> GetGenericImages() => ctx.GenericImage.ToList();
        public int GetProfilePictureId(string employeeId) => ctx.GenericImage.Select(image => image)
                                                                          .Where(image => image.UserId == employeeId)
                                                                          .First()
                                                                          .Id;
        public GenericImage GetEstateThumbnail(int estateId) => ctx.GenericImage.Where(img => img.EstateId == estateId)
                                                                                .First();
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
                                      .Where(a => a.Id == address.Id)
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
                                          .Where(e => e.Id == electricity.Id)
                                          .First();
            toUpdate.Networked = electricity.Networked;
            toUpdate.SunCollector = electricity.SunCollector;
            toUpdate.Thermal = electricity.Thermal;
        }
        
        public void UpdateHeatingSystem(HeatingSystem heating)
        {
            var toUpdate = ctx.HeatingSystem.Select(h => h)
                                            .Where(h => h.Id == heating.Id)
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
                                            .Where(p => p.Id == publicService.Id)
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
                                          .Where(w => w.Id == waterSystem.Id)
                                          .First();
            toUpdate.AvailabilityType = waterSystem.AvailabilityType;

            ctx.SaveChanges();
        }
        
        public void UpdateAdvertisement(Advertisement advertisement)
        {
            var toUpdate = ctx.Advertisement.Select(a => a)
                                            .Where(a => a.Id == advertisement.Id)
                                            .First();
            toUpdate.DescriptionDetail = advertisement.DescriptionDetail;
            toUpdate.LastModification = advertisement.LastModification;
            toUpdate.OfferType = advertisement.OfferType;
            toUpdate.OrderOfAppearance = advertisement.OrderOfAppearance;
            toUpdate.Title = advertisement.Title;

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

            var popularity = ctx.Popularity.Select(p => p)
                                           .Where(p => p.AdvertisementId == advertisement.Id)
                                           .First();

            ctx.Popularity.Remove(popularity);
            ctx.SaveChanges();

            ctx.Advertisement.Remove(advertisement);
            ctx.SaveChanges();

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

            ctx.WaterSystem.Remove(waterSystem);                                    
            ctx.PublicService.Remove(publicService);
            ctx.HeatingSystem.Remove(heatingSystem);
            ctx.Electricity.Remove(electricity);
            ctx.Address.Remove(address);
            ctx.SaveChanges();

            var images = ctx.GenericImage.Select(gi => gi)
                                         .Where(gi => gi.EstateId == estateId)
                                         .ToList();

            ctx.GenericImage.RemoveRange(images);
            ctx.SaveChanges();

            var estate = ctx.Estate.Select(e => e)
                                    .Where(e => e.Id == estateId)
                                    .First();

            ctx.Estate.Remove(estate);
            ctx.SaveChanges();

            success = true;
            return success;
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
