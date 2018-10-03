using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzUtility.CosmosDBCompass
{
    public class CosmosDownloader
    {
        private readonly DocumentClient _client;
        private readonly string _baseFolder;

        public CosmosDownloader(DocumentClient client,string baseFolder)
        {
            _client = client;
            _baseFolder = baseFolder;
        }

        public async Task RunAsync()
        {
            Console.WriteLine($"Begin downloading data to folder: {_baseFolder}");

            var databases = await _client.ReadDatabaseFeedAsync();
            foreach (var db in databases)
            {
                Console.WriteLine(db);
            }
        }
    }
}
