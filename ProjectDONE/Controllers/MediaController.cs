using Microsoft.WindowsAzure.Storage;
using ProjectDONE.Data.Repos;
using ProjectDONE.Models;
using ProjectDONE.Models.AppModels;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProjectDONE.Controllers
{
   
    [RoutePrefix("api/media")]
    public class MediaController : ApiController
    {
        private MediaRepo _MediaRepo;
        public MediaController()
        {
            _MediaRepo = new MediaRepo();
        }

        [HttpGet]
        [Route("{*path}")]
        public async Task<HttpResponseMessage> Image(string path)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var constring = ConfigurationManager.AppSettings["StorageConnectionString"];
            var acct = CloudStorageAccount.Parse(constring);
            var client = acct.CreateCloudBlobClient();
            var container = client.GetContainerReference("job-images");
            var blob = container.GetBlockBlobReference(path);
            if(!blob.Exists())
            {
                //TODO: Return a default image indicating that this image could not be found
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new ByteArrayContent(new byte[0]);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + Path.GetExtension(path).Replace(".",""));
                return response;
            }
            var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            response.Content = new ByteArrayContent(stream.ToArray());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(blob.Properties.ContentType);
            return response;
        }


        [Authorize]
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
            var guid_file_name = Guid.NewGuid().ToString() + Path.GetExtension(filename);
            var blob = container.GetBlockBlobReference(guid_file_name);
            blob.Properties.ContentType = provider.FileData[0].Headers.ContentType.MediaType;
            var fileupload = blob.UploadFromStreamAsync(File.OpenRead(provider.FileData[0].LocalFileName));

            var model = new Media
            {
                CreatedOn = DateTime.Now,
                MIME_TYPE = blob.Properties.ContentType,
                Title = Path.GetFileName(filename),
                URL = "/api/media/" + guid_file_name
            };

           
           
      
            _MediaRepo.Add(model);
            _MediaRepo.Save();
            await fileupload;
            //TODO: check for file upload errors here

            return Request.CreateResponse<MediaViewModel>((MediaViewModel)model);

        }
       
    }
}