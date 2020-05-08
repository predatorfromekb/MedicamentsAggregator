namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentModel
    {
        public MedgorodokMedicamentModel(MedgorodokPharmacyModel[] pharmacies)
        {
            Pharmacies = pharmacies;
        }

        public MedgorodokPharmacyModel[] Pharmacies { get; }
        public int Count => Pharmacies.Length;
    }
}