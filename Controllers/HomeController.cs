using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using WebAdvert.Web.Models.Adverts;
using WebAdvert.Web.ServiceClients;

namespace AspNetCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISearchApiClient _searchApiClient;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, ISearchApiClient searchApiClient, IMapper mapper)
        {
            _logger = logger;
            _searchApiClient = searchApiClient;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            var viewModel = new List<SearchViewModel>();

            var searchResult = await _searchApiClient.SearchAsync(keyword).ConfigureAwait(false);
            searchResult.ForEach(advertDoc =>
            {
                var viewModelItem = _mapper.Map<SearchViewModel>(advertDoc);
                viewModel.Add(viewModelItem);
            });

            return View("Search", viewModel);
        }
    }
}
