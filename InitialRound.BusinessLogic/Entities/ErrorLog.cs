///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace InitialRound.BusinessLogic.Entities
{
    public class ErrorLog : TableServiceEntity
    {
        public ErrorLog(string message, string username)
        {
            this.PartitionKey = Constants.DefaultPartition;
            this.RowKey = Guid.NewGuid().ToString();
            this.Message = message;
            this.Username = username;
            this.CreatedDate = DateTime.UtcNow;
        }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Username { get; set; }
    }
}
