using GeoCoordinatePortable;
using MedicamentsAggregator.Service.DataLayer.Tables;

namespace MedicamentsAggregator.Service.Models.Extensions
{
    public static class PharmacyExtensions
    {
        public static double GetDistanceTo(this Pharmacy pharmacy, GeoCoordinate to)
        {
            if (pharmacy.Latitude == null || pharmacy.Longitude == null)
                return double.MaxValue;
            return new GeoCoordinate(pharmacy.Latitude.Value, pharmacy.Longitude.Value).GetDistanceTo(to);
        }
    }
}