using AzUtility.CosmosDBCompass.Meta;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzUtility.CosmosDBCompass
{
    public class CosmosDownloader
    {
        private readonly DocumentClient _client;
        private readonly string _baseFolder;

        public CosmosDownloader(DocumentClient client, string baseFolder)
        {
            _client = client;
            _baseFolder = baseFolder;
        }

        public async Task RunAsync()
        {
            Console.WriteLine($"Begin downloading data to folder: {_baseFolder}");
            var cosmosMeta = new CosmosMeta();
            cosmosMeta.Databases = new List<DatabaseMeta>();

            var databases = await _client.ReadDatabaseFeedAsync();
            foreach (var db in databases)
            {
                Console.WriteLine($"Procerssing database: {db.Id}");
                var dbMeta = new DatabaseMeta();
                dbMeta.DBSchema = db;
                dbMeta.CollectionSchemas = new List<DocumentCollection>();

                // Create a folder for each database.
                string dbFolder = Path.Combine(_baseFolder, db.Id);
                Directory.CreateDirectory(dbFolder);

                var collections = await _client.ReadDocumentCollectionFeedAsync(db.CollectionsLink);
                foreach (var collection in collections)
                {
                    Console.WriteLine($"Procerssing collection: {collection.Id}");
                    dbMeta.CollectionSchemas.Add(collection);

                    var docs = await _client.ReadDocumentFeedAsync(collection.DocumentsLink);
                    var serilized = JsonConvert.SerializeObject(docs);

                    // Create a file for each data base.
                    string collectionFile = Path.Combine(dbFolder, collection.Id + ".json");
                    try
                    {
                        File.WriteAllText(collectionFile, serilized);
                    }
                    catch { }
                }

                cosmosMeta.Databases.Add(dbMeta);
            }

            string metaFile = Path.Combine(_baseFolder, Constants.MetaFileName);
            try
            {
                File.WriteAllText(metaFile, JsonConvert.SerializeObject(cosmosMeta));
            }
            catch { }

            Console.WriteLine($"Finished downloading data to folder: {_baseFolder}");
        }
    }
}
