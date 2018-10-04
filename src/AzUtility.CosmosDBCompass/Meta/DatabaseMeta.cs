using Microsoft.Azure.Documents;
using System.Collections.Generic;

namespace AzUtility.CosmosDBCompass.Meta
{
    public class DatabaseMeta
    {
        public Database DBSchema { get; set; }

        public List<DocumentCollection> CollectionSchemas { get; set; }
    }
}
