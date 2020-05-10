using System.Collections.Generic;
using MedicamentsAggregator.Service.DataLayer;

namespace MedicamentsAggregator.Service.Models.Common
{
    public class EntityEqualityComparer : IEqualityComparer<IFixedIdEntity>
    {
        public bool Equals(IFixedIdEntity x, IFixedIdEntity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(IFixedIdEntity obj)
        {
            return obj.Id;
        }
        
        public static IEqualityComparer<IFixedIdEntity> Instance { get; } = new EntityEqualityComparer();
    }
}