using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Auth;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace WebAdvert.Web.Services
{
    public class S3FileUploader : IFileUploader
    {
        private readonly IConfiguration _configuration;

        public S3FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(string filename, Stream storageStream)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("File name is required");

            var bucketName = _configuration.GetValue<string>("ImageBucket");

            using var client = new AmazonS3Client();
            if (storageStream.Length > 0 && storageStream.CanSeek)
                storageStream.Seek(0, SeekOrigin.Begin);

            var request = new PutObjectRequest
            {
                AutoCloseStream = true,
                BucketName = bucketName,
                InputStream = storageStream,
                Key = filename
            };
            var response = await client.PutObjectAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
    }
}
