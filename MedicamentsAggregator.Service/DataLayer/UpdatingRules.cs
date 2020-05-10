using System;
using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.DataLayer
{
    public static class UpdatingRules
    {
        public static readonly Action<Medicament, Medicament> ForMedicament = (db, n) =>
        {
            db.Url = n.Url;
            db.Title = n.Title;
        };

        public static readonly Action<Pharmacy, Pharmacy> ForPharmacy = (db, n) =>
        {
            //TODO - реализовать
//            if (db.Address != n.Address)
//            {
//                db.Latitude = null;
//                db.Longitude = null;
//            }

            db.Address = n.Address;
            db.Title = n.Title;
        };
    }
}