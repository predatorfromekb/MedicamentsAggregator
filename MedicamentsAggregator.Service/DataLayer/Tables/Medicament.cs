using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MedicamentsAggregator.Service.DataLayer.Tables
{
    [Table("Medicament")]
    public class Medicament : IFixedIdEntity
    {
        [Key]
        public int Id { get; set; }
        
        [NotNull]
        [StringLength(255)]
        public string Title { get; set; }
        
        [NotNull]
        [StringLength(255)]
        public string Url { get; set; }
        
        public DateTime UpdatedDate { get; set; }
    }
}