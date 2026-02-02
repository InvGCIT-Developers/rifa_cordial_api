using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Rifas.Client.Interfaces;
using Rifas.Client.Models.DTOs;
using Rifas.Client.Modulos.Services;
using Rifas.Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Services
{
    public class CloudflareService : ICloudflareService
    {
        private readonly string _baseUrl;
        private readonly string _cloudfareAccountId;
        private readonly string _cloudfareKey;
        private readonly string _cloudfareEmail;
        private readonly IConfiguration _configuration;
        private readonly RestClient _client;
        //private readonly IRaffleService _raffleService;

        public CloudflareService(IConfiguration _configuration)
        {
            _configuration = _configuration;

            _baseUrl = _configuration.GetValue<string>("cloudfareUrl");
            _cloudfareAccountId = _configuration.GetValue<string>("cloudfareAccountId");
            _cloudfareKey = _configuration.GetValue<string>("cloudfareKey");
            _cloudfareEmail = _configuration.GetValue<string>("cloudfareEmail");

            var options = new RestClientOptions(_baseUrl)
            {
                Timeout = TimeSpan.Zero,
            };

            _client = new RestClient(options);
            //_raffleService = raffleService;
        }

        public async Task<DownLoadImagegResponse?> UploadImageAsync(byte[] fileBytes, string fileName, long raffleId)
        {
            var request = new RestRequest($"/accounts/{_cloudfareAccountId}/images/v1", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_cloudfareKey}");
            request.AlwaysMultipartFormData = true;
            request.AddFile("file", fileBytes, fileName);
            request.AddParameter("metadata", JsonConvert.SerializeObject(new { key = fileName }), ParameterType.RequestBody);
            request.AddParameter("requireSignedURLs", "false");

            var response = await _client.ExecuteAsync<DownLoadImagegResponse>(request);
            if (!response.IsSuccessful)
            {
                throw new Exception(response.ErrorMessage);
            }

            var obj = JsonConvert.DeserializeObject<DownLoadImagegResponse>(response.Content);

            return obj;
        }



        public async Task<DownLoadImagegResponse> DownloadImageAsync(string imageId)
        {

            var request = new RestRequest($"/accounts/{_cloudfareAccountId}/images/v1/{imageId}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_cloudfareKey}");
            var response = await _client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                throw new Exception(response.ErrorMessage);
            }

            var obj = JsonConvert.DeserializeObject<DownLoadImagegResponse>(response.Content);


            return obj;
        }

        public async Task<List<DownLoadImagegResponse>> DownloadMultipleImagesAsync(List<string> imageIds)
        {
            var tasks = imageIds.Select(id => DownloadImageAsync(id)).ToList();
            return [.. (await Task.WhenAll(tasks))];
        }
    }
}
