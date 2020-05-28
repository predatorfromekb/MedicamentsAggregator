using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Context;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vostok.Logging.Abstractions;
using Yandex.Geocoder;
using Yandex.Geocoder.Enums;

namespace MedicamentsAggregator.Service.Models.GeoCoder
{
    public class PharmacyCoordinatesUpdater
    {
        private readonly IConfiguration _configuration;
        private readonly MedicamentsAggregatorContextFactory _contextFactory;
        private readonly GeoCoderLog _log;
        
        private const string YekaterinburgRegionPrefix = "Свердловская область, ";

        public PharmacyCoordinatesUpdater(IConfiguration configuration,
            MedicamentsAggregatorContextFactory contextFactory,
            GeoCoderLog log)
        {
            _configuration = configuration;
            _contextFactory = contextFactory;
            _log = log;
        }

        public async Task UpdateCoordinates(Pharmacy[] pharmacies)
        {
            var tasks = pharmacies.Select(UpdateCoordinate);
            await Task.WhenAll(tasks);
            await using (var context = _contextFactory.CreateContext())
            {
                context.AttachRange(pharmacies);
                foreach (var pharmacy in pharmacies)
                {
                    context.Entry(pharmacy).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }

        private async Task<bool> UpdateCoordinate(Pharmacy pharmacy)
        {
            var query = YekaterinburgRegionPrefix + pharmacy.City + ", " + pharmacy.Address;
            var request = new GeocoderRequest { Request = query };
            var apiKey = _configuration["GeoCoderApiKey"];
            var client = new GeocoderClient(apiKey);
            var response = await client.Geocode(request);

            var geoObjects = response.GeoObjectCollection.FeatureMember;

            if (geoObjects.Count == 0)
            {
                _log.Error($"No geoobjects by query {query} - Id {pharmacy.Id}");
                return false;
            }
            
            var geoObjectFromSelectedCity = geoObjects
                .FirstOrDefault(e =>
                    e.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components
                        .FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality))
                        ?.Name == pharmacy.City);
            
            var geoObject = geoObjectFromSelectedCity ?? geoObjects.First();
            
            if (geoObjects.Count > 1 && geoObjectFromSelectedCity == null)
            {
                _log.Warn($"Multiple geoobjects ({geoObjects.Count}) and No one from city {pharmacy.City} by query {query} - Id {pharmacy.Id}.");
            }


            var coordinate = geoObject.GeoObject.Point.Pos;
            var formattedAddress = geoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Formatted;

            pharmacy.FormattedAddress = formattedAddress.Replace("Россия, Свердловская область, ", string.Empty);

            try
            {
                UpdateCoordinates(coordinate, pharmacy);
                return true;
            }
            catch (Exception exception)
            {
                _log.Error(exception, $"query {query} - Id {pharmacy.Id}");
                return false;
            }
        }

        private void UpdateCoordinates(string coordinate, Pharmacy pharmacy)
        {
            var segments = coordinate.Split(' ');
            
            var longitudeIsParsed = double.TryParse(segments[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude);
            var latitudeIsParsed = double.TryParse(segments[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude);

            if (longitudeIsParsed)
            {
                pharmacy.Longitude = longitude;
            }
            
            if (latitudeIsParsed)
            {
                pharmacy.Latitude = latitude;
            }
        }
    }
}