using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentModel
    {
        public MedgorodokMedicamentModel(MedgorodokPharmacyModel[] pharmacies, Medicament medicament)
        {
            Pharmacies = pharmacies;
            Medicament = medicament;
        }

        public MedgorodokPharmacyModel[] Pharmacies { get; }
        public Medicament Medicament { get; }
        public int Count => Pharmacies.Length;
    }
}