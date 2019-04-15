using System;
using System.IO;
using System.Threading.Tasks;
using blip.webhookreceiver.core.Interfaces;
using blip.webhookreceiver.core.Models.Output;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace blip.webhookreceiver.bigquery.Services
{
    public class BigQueryMessageRespository : IMessageRepository
    {

        private readonly BigQueryTable _table;
        private readonly ILogger _logger;


        public BigQueryMessageRespository(ILogger<BigQueryMessageRespository> logger)
        {
            // Get projectId fron config
            string googleCredentialsText = File.ReadAllText(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            JObject googleCredentials = JObject.Parse(googleCredentialsText);
            string projectId = googleCredentials["project_id"].ToString();

            // Get Dataset Name and Table Name
            string datasetName = Environment.GetEnvironmentVariable("DATASET_NAME");
            string tableName = Environment.GetEnvironmentVariable("MESSAGE_TABLE_NAME");

            // Get Table and Client
            BigQueryClient client = BigQueryClient.Create(projectId);
            BigQueryDataset dataset = client.GetDataset(datasetName);

            _logger = logger;
            _table = dataset.GetTable(tableName);
            _logger.LogInformation("GCP Information set. projectId: {projectId} datasetName: {datasetName},tableName:{tableName}, ", projectId, datasetName, tableName);

        }
        public void SaveMessage(OutputMessage ouputMessage)
        {
            try
            {
                _table.InsertRow(new BigQueryInsertRow
                    {
                        { "botIdentifier", ouputMessage.botIdentifier},
                        { "type",  ouputMessage.type },
                        { "id",  ouputMessage.id },
                        { "from",  ouputMessage.from},
                        { "to",  ouputMessage.to},
                        { "metadata",  ouputMessage.metadata},
                        { "content",  ouputMessage.content},
                        { "target",  ouputMessage.target},
                        { "uri",  ouputMessage.uri},
                        { "previewUri",  ouputMessage.previewUri},
                        { "title",  ouputMessage.title},
                        { "text",  ouputMessage.text},
                        { "storageDate",  ouputMessage.storageDate.ToUniversalTime().ToString()},
                    }
               );
                _logger.LogInformation("Inserted to Messages. ouputMessage: {ouputMessage}", JsonConvert.SerializeObject(ouputMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}