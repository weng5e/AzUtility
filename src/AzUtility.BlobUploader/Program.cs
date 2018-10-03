using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AzUtility.BlobUploader
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-d", "Relative or absolute path to the folder that you want to upload.", CommandOptionType.SingleValue)]
        [Required]
        public string Dir { get; }

        [Option("-c", "Storage connection string. Can be ignored when '-l' is specified.", CommandOptionType.SingleValue)]
        public string ConnectionString { get; set; }

        [Option("-l", "Use local storage emulator.", CommandOptionType.NoValue)]
        public bool Local { get; }

        public void OnExecute()
        {
            Console.WriteLine($"Use local storage emulator: {Local}");
            if (Local && string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            }

            if (CloudStorageAccount.TryParse(ConnectionString, out var storageAccount))
            {
                var fullDir = Path.GetFullPath(Dir);
                Console.WriteLine($"The folder to upload is: {fullDir}");
                var uploader = new FolderUploader(storageAccount, fullDir);
                uploader.RunAsync().Wait();
            }
            else
            {
                Console.Error.WriteLine("Connection String is invalid.");
            }
        }
    }
}
