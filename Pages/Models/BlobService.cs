using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace MusicLibrary.Models
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobClient;

        private readonly string containerName = "songs";

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<bool> DeleteFile(string name)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetFile(string name)
        {
            // this will allow us access to the storage container
            var containerClient = _blobClient.GetBlobContainerClient(containerName);

            // this will allow us access to the file inside the container via the file name
            var blobClient = containerClient.GetBlobClient(name);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadFile (string name, IFormFile file)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);

            // checking if the file exist 
            // if the file exist it will be replaced
            // if it doesn't exist it will create a temp space until its uploaded
            var blobClient = containerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            var res = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

            if (res != null)
                return true;

            return false;
        }
        public async Task<IEnumerable<string>> AllFiles()
        {
            // allow us to access the data inside the container
            var containerClient = _blobClient.GetBlobContainerClient(containerName);

            var files = new List<string>();

            var blobs = containerClient.GetBlobsAsync();

            await foreach (var item in blobs)
            {
                files.Add(item.Name);
            }

            return files;
        }
    }

}
