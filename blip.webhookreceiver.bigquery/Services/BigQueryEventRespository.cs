using System;
using System.IO;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Output;
using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.bigquery.Services
{
    public class BigQueryEventRespository : IEventRepository
    {
        private readonly BigQueryTable _table;

        public BigQueryEventRespository()
        {
            // Get projectId fron config
            string googleCredentialsText = File.ReadAllText(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            JObject googleCredentials = JObject.Parse(googleCredentialsText);
            string projectId = googleCredentials["project_id"].ToString();

            // Get Dataset Name and Table Name
            string datasetName = Environment.GetEnvironmentVariable("DATASET_NAME");
            string tableName = Environment.GetEnvironmentVariable("EVENT_TABLE_NAME");

            // Get Table and Client
            BigQueryClient client = BigQueryClient.Create(projectId);
            BigQueryDataset dataset = client.GetDataset(datasetName);
            BigQueryTable _table = dataset.GetTable(tableName);
        }
        public async Task SaveEvent(OutputEvent outputEvent)
        {
            await _table.InsertRowAsync(new BigQueryInsertRow
                {
                    { "botIdentifier", outputEvent.botIdentifier},
                    { "ownerIdentity",  outputEvent.ownerIdentity },
                    { "identity",  outputEvent.identity },
                    { "messageId",  outputEvent.messageId },
                    { "storageDate",  outputEvent.storageDate },
                    { "category",  outputEvent.category },
                    { "action",  outputEvent.action },
                    { "extras",  outputEvent.extras },
                    { "externalId",  outputEvent.externalId },
                    { "group",  outputEvent.group },
                    { "source",  outputEvent.source },
                    { "value",  outputEvent.value },
                    { "label",  outputEvent.label }
                }
            );
        }
    }
}