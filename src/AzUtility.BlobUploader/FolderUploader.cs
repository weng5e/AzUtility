using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzUtility.BlobUploader
{
    public class FolderUploader
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly string _baseFolder;

        public FolderUploader(CloudStorageAccount storageAccount, string baseFolder)
        {
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
            _baseFolder = baseFolder;
        }

        public async Task RunAsync()
        {
            var containers = Directory.GetDirectories(_baseFolder);
            foreach (var containerDir in containers)
            {
                var containerName = new DirectoryInfo(containerDir).Name;
                Console.WriteLine($"Check and create container: {containerName}");

                var cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();

                string[] files = Directory.GetFiles(containerDir, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    Console.WriteLine($"[Begin] Container: {containerName} Upload File:{file}");
                    var relativePath = file.Substring(containerDir.Length + 1);
                    var blobName = relativePath.Replace('\\', '/');

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
                    try
                    {
                        var exist = await cloudBlockBlob.ExistsAsync();
                        if (!exist)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                try
                                {
                                    await cloudBlockBlob.UploadFromFileAsync(file);
                                    Console.WriteLine($"[Finish] Container: {containerName} Upload File:{file}");
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }
        }
    }
}
