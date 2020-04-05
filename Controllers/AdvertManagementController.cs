using System;
using System.IO;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
            _mapper = mapper;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
                var id = apiCallResponse.Id;

                if (imageFile != null)
                {
                    var fileName = !string.IsNullOrWhiteSpace(imageFile.FileName)
                        ? Path.GetFileName(imageFile.FileName)
                        : id;
                    string filePath = $"{id}/{fileName}";

                    try
                    {
                        await using var readStream = imageFile.OpenReadStream();
                        var result = await _fileUploader.UploadFileAsync(filePath, readStream).ConfigureAwait(false);
                        if (!result)
                            throw new Exception("Failed to upload image to file repository");

                        var confirmModel = new ConfirmAdvertRequest
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };

                        bool canConfirm = await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                        if (!canConfirm)
                            throw new Exception($"Cannot confirm advert of id = {id}");

                        return RedirectToAction("Index", "Home");
                    }
                    catch(Exception e)
                    {
                        var confirmModel = new ConfirmAdvertRequest
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };

                        await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                        Console.WriteLine(e);
                    }
                }
            }

            return View(model);
        }
    }
}