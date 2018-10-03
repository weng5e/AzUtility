using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

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
            }
            else
            {
                Console.Error.WriteLine("Connection String is invalid.");
            }
        }
    }
}
