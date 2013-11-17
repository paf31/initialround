///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using InitialRound.Common.Extensions;
using InitialRound.BusinessLogic.Classes;
using System.Data.Services.Client;
using System.Data;
using System.Data.SqlClient;
using InitialRound.Models.Contexts;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class DataController
    {
        #region Table Storage
        public static void CreateTablesIfNecessary()
        {
            string tableStorageConnectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("TableStorageConnectionString");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.GetTableReference(Constants.ErrorLog.Name).CreateIfNotExists();
        }

        public static TableServiceContext CreateServiceContext()
        {
            string tableStorageConnectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("TableStorageConnectionString");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            return tableClient.GetTableServiceContext();
        }
        #endregion

        #region SQL Azure
        public static Models.Contexts.DbContext CreateDbContext(string connectionString)
        {
            Models.Contexts.DbContext context = new Models.Contexts.DbContext(connectionString);

            ((IObjectContextAdapter)context).ObjectContext.Connection.Open();

            return context;
        }

        public static Models.Contexts.DbContext CreateDbContext()
        {
            string connectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("RootConnectionString");

            return CreateDbContext(connectionString);
        }

        public static SqlConnection CreateSqlConnection()
        {
            string connectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("RootConnectionString");

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();

            return connection;
        }
        #endregion

        #region Queue Storage
        public static void CreateQueuesIfNecessary()
        {
            GetEmailQueue().CreateIfNotExists();
        }

        public static CloudQueue GetEmailQueue()
        {
            string queueStorageConnectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("QueueStorageConnectionString");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            return queueClient.GetQueueReference(Constants.EmailQueue);
        }
        #endregion

        #region Blob Storage
        public static CloudBlobClient CreateBlobClient()
        {
            string blobStorageConnectionString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("BlobStorageConnectionString");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);

            return storageAccount.CreateCloudBlobClient();
        }

        public static CloudBlobContainer GetBlobContainer()
        {
            CloudBlobClient blobClient = CreateBlobClient();

            return blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("BlobContainerName"));
        }

        public static ICloudBlob GetBlob(string name)
        {
            CloudBlobContainer container = GetBlobContainer();

            return container.GetBlobReferenceFromServer(name);
        }

        public static void UploadBlob(string name, string value)
        {
            ICloudBlob blob = GetBlob(name);

            if (value == null)
            {
                value = string.Empty;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(value);
                }

                blob.UploadFromStream(stream);
            }
        }

        public static string DownloadBlob(string name)
        {
            ICloudBlob blob = GetBlob(name);

            try
            {
                blob.FetchAttributes();

                using (MemoryStream stream = new MemoryStream())
                { 
                    blob.DownloadToStream(stream);

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 404)
                {
                    return string.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        public static void DeleteBlob(string name)
        {
            ICloudBlob blob = GetBlob(name);

            blob.DeleteIfExists();
        }
        #endregion
    }
}
