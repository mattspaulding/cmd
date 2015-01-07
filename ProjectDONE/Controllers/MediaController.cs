using System;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;

using Microsoft.WindowsAzure.Storage;

using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using ProjectDONE.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace ProjectDONE.Controllers
{
    [RoutePrefix("api/media")]
    public class MediaController : ApiController
    {
        [HttpGet]
        [Route("*{path}")]
        public HttpResponseMessage Get(string path)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(path ?? "");
            return response;
        }
       [HttpPost]
       [Route("Upload")]
        public async Task<HttpResponseMessage> Upload()
        {


            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            
            var provider = new MultipartFileStreamProvider(Path.GetTempPath());
            await request.Content.ReadAsMultipartAsync(provider);

            var filename = provider.FileData[0].Headers.ContentDisposition.FileName.Trim('"', '\\');

            var constring = ConfigurationManager.AppSettings["StorageConnectionString"];
            var acct = CloudStorageAccount.Parse(constring);
            var client = acct.CreateCloudBlobClient();
            var container = client.GetContainerReference("job-images");
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + Path.GetExtension(filename));
            blob.Properties.ContentType = provider.FileData[0].Headers.ContentType.MediaType;

            var model = new MediaViewModel
            {
                CreatedOn = DateTime.Now,
                MIME_TYPE = blob.Properties.ContentType,
                Title = Path.GetFileName(filename)
            };

            blob.UploadFromStream(File.OpenRead(provider.FileData[0].LocalFileName));
            model.URL = blob.Uri.AbsoluteUri;

            return Request.CreateResponse<MediaViewModel>(model);

        }
       
    }
}