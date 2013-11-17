using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.Common.Classes;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace InitialRound.Common.Controllers
{
    public static class DataController
    {
        public static void Insert<TEntity>(TableServiceContext serviceContext, EntitySetName<TEntity> entitySetName, TEntity entity) where TEntity : TableServiceEntity
        {
            serviceContext.AddObject(entitySetName.Name, entity);
            serviceContext.SaveChanges();
        }

        public static IQueryable<TEntity> Query<TEntity>(TableServiceContext serviceContext, EntitySetName<TEntity> entitySetName, string partitionKey, string fromRowKey = null) where TEntity : TableServiceEntity
        {
            IQueryable<TEntity> allEntities = serviceContext.CreateQuery<TEntity>(entitySetName.Name)
                .Where(a => a.PartitionKey == partitionKey);

            if (fromRowKey != null)
            {
                return allEntities.Where(a => a.RowKey.CompareTo(fromRowKey) > 0);
            }

            return allEntities;
        }

        public static TEntity Get<TEntity>(TableServiceContext serviceContext, EntitySetName<TEntity> entitySetName, string partitionKey, string rowKey) where TEntity : TableServiceEntity
        {
            return serviceContext.CreateQuery<TEntity>(entitySetName.Name)
                .Where(a => a.PartitionKey == partitionKey)
                .Where(a => a.RowKey == rowKey)
                .FirstOrDefault();
        }
    }
}
