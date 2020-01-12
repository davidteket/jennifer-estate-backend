using DunakanyarHouseIngatlan.DataAccess.Entities;

using System.Collections.Generic;

namespace DunakanyarHouseIngatlan.DataAccess
{
    public interface IRepository
    {
        # region QUERY

        Address GetAddress(int estateId);

        List<Advertisement> GetAdvertisements();

        Advertisement GetAdvertisement(int estateId);

        int CountEstates();

        List<ClientRequest> GetClients();

        List<Electricity> GetElectricities();

        Electricity GetElectricity(int estateId);

        PublicService GetPublicService(int estateId);

        Estate GetEstate(int id);

        List<Estate> GetEstatesSimpleCriteria(string estateType, int minPrice, int maxPrice, string city);

        List<Estate> GetEstates(int? fromId, int limit);

        List<EstateClient> GetEstateClients();

        List<GenericImage> GetEstateImages(int estateId);

        GenericImage GetProfilePictureId(string employeeId);

        GenericImage GetEstateThumbnail(int estateId);

        List<HeatingSystem> GetHeatingSystems();

        HeatingSystem GetHeatingSystem(int estateId);

        byte[] GetImage(int imageId);

        List<string> GetRandomImages(int count);

        #endregion
        #region UPDATE
        
        void UpdateEstate(Estate estate);

        void UpdateAddress(Address address);

        void UpdateElectricity(Electricity electricity);

        void UpdateHeatingSystem(HeatingSystem heating);

        void UpdatePublicService(PublicService publicService);

        void UpdateAdvertisement(Advertisement advertisement);

        #endregion
        #region INSERT

        int AddEstate(Estate estate);

        void AddAddress(Address address, int estateId);

        void AddElectricity(Electricity electricity, int estateId);

        void AddHeatingSystem(HeatingSystem heatingSystem, int estateId);

        void AddPublicService(PublicService publicService, int estateId);

        void AddAdvertisement(Advertisement advertisement, int estateId, string advertiserId);

        bool AddImage(GenericImage image);

        void AddClientRequest(ClientRequest clientRequest);

        #endregion
        #region DELETE

        bool DeleteEstate(int estateId);
        
        bool DeleteAllEntries();
        
        #endregion
    }
}
