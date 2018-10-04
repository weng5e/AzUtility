using AzUtility.CosmosDBCompass.Meta;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzUtility.CosmosDBCompass
{
    public class CosmosUploader
    {
        private readonly DocumentClient _client;
        private readonly string _baseFolder;

        public CosmosUploader(DocumentClient client, string baseFolder)
        {
            _client = client;
            _baseFolder = baseFolder;
        }

        public async Task RunAsync()
        {
            Console.WriteLine($"Begin uploading data from folder: {_baseFolder}");
            string metaFile = Path.Combine(_baseFolder, Constants.MetaFileName);
            var meta = JsonConvert.DeserializeObject<CosmosMeta>(File.ReadAllText(metaFile));

            var databaseFolders = Directory.GetDirectories(_baseFolder);
            foreach (var dbDir in databaseFolders)
            {
                var dbName = new DirectoryInfo(dbDir).Name;
                Console.WriteLine($"Procerssing database: {dbName}");
                var dbMeta = meta.Databases.Where(d => d.DBSchema?.Id == dbName).FirstOrDefault();
                if (dbMeta == null)
                {
                    throw new Exception("Meta data cannot be found.");
                }

                ResourceResponse<Database> databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(dbMeta.DBSchema);

                string[] collectionFiles = Directory.GetFiles(dbDir, "*.*", SearchOption.AllDirectories);
                foreach (var collectionFile in collectionFiles)
                {
                    var collectionName = Path.GetFileNameWithoutExtension(collectionFile);
                    Console.WriteLine($"Procerssing collection: {collectionName}");
                    var colMeta = dbMeta.CollectionSchemas.Where(c => c.Id == collectionName).FirstOrDefault();
                    if (colMeta == null)
                    {
                        throw new Exception("Meta data cannot be found.");
                    }

                    ResourceResponse<DocumentCollection> collectionResponse = await _client.CreateDocumentCollectionIfNotExistsAsync(
                    databaseResponse.Resource.SelfLink,
                    colMeta,
                    new RequestOptions
                    {
                        OfferThroughput = 1000,
                    });

                    string collectionFileContent = File.ReadAllText(collectionFile);
                    try
                    {
                        dynamic docs = JsonConvert.DeserializeObject(collectionFileContent);
                        foreach (var doc in docs)
                        {
                            try
                            {
                                var response = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, collectionName), doc);
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine($"Finished uploading data from folder: {_baseFolder}");
        }
    }
}
