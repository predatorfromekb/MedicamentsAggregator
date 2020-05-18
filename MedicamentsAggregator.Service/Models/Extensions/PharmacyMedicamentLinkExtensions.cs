using System.Collections.Generic;
using GeoCoordinatePortable;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Request;

namespace MedicamentsAggregator.Service.Models.Extensions
{
    public static class PharmacyMedicamentLinkExtensions
    {
        public static IEnumerable<PharmacyMedicamentLink> FilterByRange(this IEnumerable<PharmacyMedicamentLink> links, RequestAggregateSettings settings)
        {
            return settings.UseSearchRadius ? links.FilterByRangeInternal(settings) : links;
        }

        private static IEnumerable<PharmacyMedicamentLink> FilterByRangeInternal(
            this IEnumerable<PharmacyMedicamentLink> links, RequestAggregateSettings settings)
        {
            var clientCoordinates = new GeoCoordinate(settings.Latitude, settings.Longitude);
            foreach (var link in links)
            {
                if (link.Pharmacy.GetDistanceTo(clientCoordinates) <= settings.SearchRadius)
                {
                    yield return link;
                }
            }
        } 
    }
}