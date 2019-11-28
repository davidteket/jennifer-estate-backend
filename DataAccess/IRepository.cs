using backend.DataAccess.Entities;

using System.Collections.Generic;

namespace backend.DataAccess
{
    interface IRepository
    {
        # region QUERY

        Address GetAddress(int estateId);
        List<Advertisement> GetAdvertisements();
        Advertisement GetAdvertisement(int estateId);
        List<Client> GetClients();
        List<Electricity> GetElectricities();
        Electricity GetElectricity(int estateId);
        PublicService GetPublicService(int estateId);
        Estate GetEstate(int id);
        List<Estate> GetEstates(int from, int limit);
        List<EstateClient> GetEstateClients();
        List<GenericImage> GetGenericImages();
        int GetProfilePictureId(string employeeId);
        GenericImage GetEstateThumbnail(int estateId);
        List<HeatingSystem> GetHeatingSystems();
        HeatingSystem GetHeatingSystem(int estateId);
        List<WaterSystem> GetWaterSystems();
        CompanyDetails GetCompanyDetails();
        byte[] GetImage(int imageId);

        bool CheckInvitationStatus(Invitation invitation);

        #endregion
        #region UPDATE
        
        void UpdateEstate(Estate estate);
        void UpdateAddress(Address address);
        void UpdateElectricity(Electricity electricity);
        void UpdateHeatingSystem(HeatingSystem heating);
        void UpdatePublicService(PublicService publicService);
        void UpdateWaterSystem(WaterSystem waterSystem);
        void UpdateAdvertisement(Advertisement advertisement);
        void InvalidateInvitationTicket(string invitationId);

        #endregion
        #region INSERT

        int AddEstate(Estate estate);
        void AddAddress(Address address, int estateId);
        void AddElectricity(Electricity electricity, int estateId);
        void AddHeatingSystem(HeatingSystem heatingSystem, int estateId);
        void AddPublicService(PublicService publicService, int estateId);
        void AddWaterSystem(WaterSystem waterSystem, int estateId);
        void AddAdvertisement(Advertisement advertisement, int estateId, string advertiserId);
        void AddInvitation(string inviteeId);

        #endregion
        #region DELETE

        bool DeleteEstate(int estateId);
        
        #endregion
    }
}
