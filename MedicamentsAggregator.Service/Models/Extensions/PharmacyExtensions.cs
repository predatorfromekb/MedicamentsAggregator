using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        
        public static Pharmacy TryGetCityFromTitle(this Pharmacy pharmacy)
        {
            foreach (var city in Cities)
            {
                if (pharmacy.Title.ToLower().Contains(city.Key))
                {
                    pharmacy.City = city.Value;
                    break;
                }
            }
            return pharmacy;
        }
        
        public static Pharmacy RemoveBracketsFromTitle(this Pharmacy pharmacy)
        {
            var title = pharmacy.Title;
            if (title != null)
            {
                var matches = BracketsRegex.Matches(title);
                foreach (var match in matches)
                {
                    title = title.Replace(match.ToString(), string.Empty);
                }

                pharmacy.Title = title.Trim();
            }
            return pharmacy;
        }

        private static Regex BracketsRegex = new Regex("\\(.+?\\)", RegexOptions.Compiled);

        private static Dictionary<string, string> Cities => new Dictionary<string, string>()
        {
            ["пышма"] = "Верхняя Пышма",
            ["среднеуральск"] = "Среднеуральск",
            ["первоуральск"] = "Первоуральск",
            ["ревда"] = "Ревда"
        };
    }
}