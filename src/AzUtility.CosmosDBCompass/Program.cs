using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;

namespace AzUtility.CosmosDBCompass
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-b", "Backup.", CommandOptionType.NoValue)]
        public bool Backup { get; }

        [Option("-d", "Relative or absolute path to the folder that you want to upload or download to.", CommandOptionType.SingleValue)]
        [Required]
        public string Dir { get; }

        [Option("-c", "CosmosDB connection string.", CommandOptionType.SingleValue)]
        [Required]
        public string ConnectionString { get; set; }

        [Option("-k", "Block and do not exit.", CommandOptionType.NoValue)]
        public bool Block { get; }

        public void OnExecute()
        {
            var mode = Backup ? "Download" : "Upload";
            Console.WriteLine($"Mode: {mode}");

            if (DocumentDbAccount.TryParse(ConnectionString, out var client))
            {
                var fullDir = Path.GetFullPath(Dir);
                if (Backup)
                {
                    var downloader = new CosmosDownloader(client, fullDir);
                    downloader.RunAsync().Wait();
                }
                else
                {
                    var uploader = new CosmosUploader(client, fullDir);
                    uploader.RunAsync().Wait();
                }

                if (Block)
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            else
            {
                Console.Error.WriteLine("Connection String is invalid.");
            }
        }
    }
}
