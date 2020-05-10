using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MedicamentsAggregator.Service.DataLayer.Tables
{
    [Table("PharmacyMedicamentLink")]
    public class PharmacyMedicamentLink : IFixedIdEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Pharmacy))]
        public int PharmacyId { get; set; }
        
        [ForeignKey(nameof(Medicament))]
        public int MedicamentId { get; set; }
        
        [NotNull]
        public double Price { get; set; }
        
        public DateTime UpdatedDate { get; set; }
        
        public Pharmacy Pharmacy { get; set; }
        public Medicament Medicament { get; set; }
    }
}