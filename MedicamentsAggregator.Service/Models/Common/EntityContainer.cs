using System.Collections.Generic;
using System.Linq;
using MedicamentsAggregator.Service.DataLayer;

namespace MedicamentsAggregator.Service.Models.Common
{
    public class EntityContainer<T> where T: class, IFixedIdEntity
    {
        public T[] InsertedEntities { get; set; }
        public T[] UpdatedEntities { get; set; }

        public IEnumerable<T> AllEntities => InsertedEntities.Concat(UpdatedEntities);
    }
}