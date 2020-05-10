using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MedicamentsAggregator.Service.DataLayer.Tables
{
    [Table("Pharmacy")]
    public class Pharmacy : IFixedIdEntity
    {
        [Key]
        public int Id { get; set; }
        
        [NotNull]
        [StringLength(255)]
        public string Title { get; set; }
        
        [NotNull]
        [StringLength(255)]
        public string Address { get; set; }

        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
        
        public DateTime UpdatedDate { get; set; }
    }
}