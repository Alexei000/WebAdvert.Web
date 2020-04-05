using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AdvertApi.Models;
using Microsoft.Extensions.Configuration;
using WebAdvert.Web.Models.Adverts;

namespace WebAdvert.Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient _client;
        private readonly string _baseAddress;

        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _baseAddress = configuration.GetSection("SearchApi").GetValue<string>("url");
        }

        public async Task<List<AdvertType>> SearchAsync(string keyword)
        {
            var result = new List<AdvertType>();
            string callUrl = $"{_baseAddress}/search/v1/{keyword}";
            var httpResponse = await _client.GetAsync(new Uri(callUrl)).ConfigureAwait(false);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var allAdverts = await httpResponse.Content.ReadAsAsync<List<AdvertType>>().ConfigureAwait(false);
                result.AddRange(allAdverts);
            }

            return result;
        }
    }
}
