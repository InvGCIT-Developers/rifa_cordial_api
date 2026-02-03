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
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

        public CloudflareService(IConfiguration configuration)
        {
            _configuration = configuration;

            _baseUrl = _configuration.GetValue<string>("cloudfareUrl");
            _cloudfareAccountId = _configuration.GetValue<string>("cloudfareAccountId");
            _cloudfareKey = _configuration.GetValue<string>("cloudfareKey");
            _cloudfareEmail = _configuration.GetValue<string>("cloudfareEmail");

            var options = new RestClientOptions(_baseUrl)
            {
                // No uses TimeSpan.Zero (puede causar cancelación inmediata).
                // Para desactivar timeout explícitamente:
                Timeout = System.Threading.Timeout.InfiniteTimeSpan
            };

            _client = new RestClient(options);
        }

        public async Task<DownLoadImagegResponse?> UploadImageAsync(byte[] fileBytes, string fileName, long raffleId)
        {
            var request = new RestRequest($"/accounts/{_cloudfareAccountId}/images/v1", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_cloudfareKey}");
            request.AlwaysMultipartFormData = true;
            request.AddFile("file", fileBytes, fileName);
            request.AddParameter("metadata", JsonConvert.SerializeObject(new { key = fileName }), ParameterType.RequestBody);
            request.AddParameter("requireSignedURLs", "false");

            RestResponse response;
            try
            {
                // Ejecutar la petición y capturar cancelaciones/timeout
                response = await _client.ExecuteAsync(request);
            }
            catch (TaskCanceledException tex)
            {
                // Timeout o cancelación
                throw new TimeoutException("Timeout al intentar subir la imagen a Cloudflare.", tex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al ejecutar la petición a Cloudflare.", ex);
            }

            if (!response.IsSuccessful)
            {
                var details = $"Status: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}, Content: {response.Content}";
                throw new Exception($"Cloudflare Upload failed: {details}");
            }

            var obj = JsonConvert.DeserializeObject<DownLoadImagegResponse>(response.Content);

            return obj;
        }

        public async Task<DownLoadImagegResponse> DownloadImageAsync(string imageId)
        {
            var request = new RestRequest($"/accounts/{_cloudfareAccountId}/images/v1/{imageId}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_cloudfareKey}");

            RestResponse response;
            try
            {
                response = await _client.ExecuteAsync(request);
            }
            catch (TaskCanceledException tex)
            {
                throw new TimeoutException("Timeout al intentar descargar la imagen de Cloudflare.", tex);
            }

            if (!response.IsSuccessful)
            {
                var details = $"Status: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}, Content: {response.Content}";
                throw new Exception($"Cloudflare Download failed: {details}");
            }

            var obj = JsonConvert.DeserializeObject<DownLoadImagegResponse>(response.Content);

            return obj;
        }

        public async Task<List<DownLoadImagegResponse>> DownloadMultipleImagesAsync(List<string> imageIds)
        {
            if (imageIds == null || imageIds.Count == 0) return new List<DownLoadImagegResponse>();

            var tasks = imageIds.Select(id => DownloadImageAsync(id)).ToArray();

            try
            {
                var results = await Task.WhenAll(tasks);
                return results.ToList();
            }
            catch (Exception ex)
            {
                // Re-lanzar o manejar según necesidad
                throw new Exception("Error al descargar imágenes múltiples desde Cloudflare.", ex);
            }
        }
    }
}
