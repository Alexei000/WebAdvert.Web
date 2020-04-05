using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Adverts;

namespace WebAdvert.Web.ServiceClients
{
    public interface ISearchApiClient
    {
        Task<List<AdvertType>> SearchAsync(string keyword);
    }
}
