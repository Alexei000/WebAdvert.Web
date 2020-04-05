using System.Threading.Tasks;
using WebAdvert.Web.Models.AdvertManagement;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertResponse> CreateAsync(CreateAdvertModel model);
        Task<bool> ConfirmAsync(ConfirmAdvertRequest model);
    }
}