using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace InitialRound.Common.Classes
{
    public class EntitySetName<TEntity> where TEntity : TableServiceEntity
    {
        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        public EntitySetName(string name)
        {
            this.name = name;
        }
    }
}
