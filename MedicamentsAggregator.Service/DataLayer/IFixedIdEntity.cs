using System;

namespace MedicamentsAggregator.Service.DataLayer
{
    public interface IFixedIdEntity
    {
        int Id { get; set; }
        DateTime UpdatedDate { get; set; }
    }
}