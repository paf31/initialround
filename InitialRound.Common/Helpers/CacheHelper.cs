using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InitialRound.Common.Helpers
{
    public class Cache<TEntity>
    {
        private readonly IQueryable<TEntity> query;
        private readonly TimeSpan timeSpan;
        private readonly object @lock = new object();
        private DateTime lastReadTime;
        private IEnumerable<TEntity> values;

        public Cache(IQueryable<TEntity> query, TimeSpan timeSpan)
        {
            this.query = query;
            this.timeSpan = timeSpan;
            this.values = query.ToList();
            this.lastReadTime = DateTime.UtcNow;
        }

        public IEnumerable<TEntity> Get()
        {
            if (lastReadTime < DateTime.UtcNow - timeSpan)
            {
                if (Monitor.TryEnter(@lock))
                {
                    try
                    {
                        values = query.ToList();
                        lastReadTime = DateTime.UtcNow;
                    }
                    finally
                    {
                        Monitor.Exit(@lock);
                    }
                }
            }

            return values;
        }
    }

    public static class CacheHelper
    {
        public static Cache<TEntity> CacheFor<TEntity>(IQueryable<TEntity> query, TimeSpan timeSpan)
        {
            return new Cache<TEntity>(query, timeSpan);
        }
    }
}
