using AdvertApi.Models;
using AutoMapper;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.AutomapperProfiles
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<AdvertModel, CreateAdvertModel>();
            CreateMap<CreateAdvertResponse, AdvertResponse>();
            CreateMap<ConfirmAdvertRequest, ConfirmAdvertModel>();
        }
    }
}
