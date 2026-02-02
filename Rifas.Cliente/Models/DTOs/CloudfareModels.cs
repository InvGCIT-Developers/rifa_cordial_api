using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifas.Client.Models.DTOs
{
    public class CloudflareImage
    {
        public string Id { get; set; }
        public string Filename { get; set; }

    }

    public class CloudflareImageListResponse
    {
        public List<CloudflareImage> Result { get; set; }
    }

    public class UploadResponse
    {
        Result result { get; set; }
        bool success { get; set; }
        object[] errors { get; set; }
        object[] messages { get; set; }
    }

    public class Result
    {
        string id { get; set; }
        string filename { get; set; }
        string uploaded { get; set; }
        bool requireSignedURLs { get; set; }
        string[] variants { get; set; }
    }

    public class ResultImage
    {
        public string id { get; set; }
        public string filename { get; set; }
        public DateTime uploaded { get; set; }
        public bool requireSignedURLs { get; set; }
        public List<string> variants { get; set; }
    }

    public class DownLoadImagegResponse
    {
        public ResultImage result { get; set; }
        public bool success { get; set; }
        public List<object> errors { get; set; }
        public List<object> messages { get; set; }
    }
}
