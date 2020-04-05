using AutoMapper;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.AutomapperProfiles
{
    public class WebSiteProfile : Profile
    {
        public WebSiteProfile()
        {
            CreateMap<CreateAdvertViewModel, CreateAdvertModel>().ReverseMap();
        }
    }
}
