using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Common;
using MedicamentsAggregator.Service.DataLayer;
using MedicamentsAggregator.Service.DataLayer.Context;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Extensions;
using MedicamentsAggregator.Service.Models.Request;
using MedicamentsAggregator.Service.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MedicamentsAggregator.Service.Models.Aggregate
{
    public class Aggregator
    {
        private readonly MedicamentsAggregatorContextFactory _contextFactory;

        public Aggregator(MedicamentsAggregatorContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ResponseAggregateModel> Aggregate(RequestAggregateModel requestAggregateModel)
        {
            

            double totalPrice = 0;

            var pharmaciesList = GetPharmacies(requestAggregateModel, ref totalPrice);

            var coordinates = pharmaciesList.Values
                .GroupBy(e => (e.Latitude, e.Longitude, e.Address),
                    (e, i) => new ResponseCoordinateModel(i.ToList(), e.Latitude, e.Longitude, e.Address))
                .ToList();
            return new ResponseAggregateModel(coordinates, totalPrice);
        }

        private Dictionary<int, ResponsePharmacyModel> GetPharmacies(RequestAggregateModel requestAggregateModel,  ref double totalPrice)
        {
            var pharmacyMedicamentLinks = GetData(requestAggregateModel);
            var settings = requestAggregateModel.Settings;
            if (!settings.LimitedPharmaciesCount)
                return GetUnlimitedPharmacies(requestAggregateModel, pharmacyMedicamentLinks, ref totalPrice);
            if (settings.PharmaciesCount == 1)
                return GetOnePharmacy(requestAggregateModel, pharmacyMedicamentLinks, ref totalPrice);
            if (settings.PharmaciesCount == 2)
                return GetTwoPharmacies(requestAggregateModel, pharmacyMedicamentLinks, ref totalPrice);
            if (settings.PharmaciesCount == 3)
                return GetThreePharmacies(requestAggregateModel, pharmacyMedicamentLinks, ref totalPrice);
            
            return GetUnlimitedPharmacies(requestAggregateModel, pharmacyMedicamentLinks, ref totalPrice);
        }

        private static Dictionary<int, ResponsePharmacyModel> GetUnlimitedPharmacies(RequestAggregateModel requestAggregateModel, 
            PharmacyMedicamentLink[] pharmacyMedicamentLinks, ref double totalPrice)
        {
            var pharmaciesList = new Dictionary<int, ResponsePharmacyModel>();
            foreach (var medicament in requestAggregateModel.Medicaments)
            {
                var cheapestLink = pharmacyMedicamentLinks
                    .Where(e => e.MedicamentId == medicament.Id)
                    .OrderBy(e => e.Price)
                    .FirstOrDefault();
                if (cheapestLink == null)
                    continue;
                if (!pharmaciesList.ContainsKey(cheapestLink.PharmacyId))
                {
                    var pharmacy = cheapestLink.Pharmacy;
                    var model = new ResponsePharmacyModel(pharmacy.Id, pharmacy.Title, pharmacy.FormattedAddress, 
                        pharmacy.Latitude.Value, pharmacy.Longitude.Value, new List<ResponseMedicamentModel>());
                    pharmaciesList.Add(cheapestLink.PharmacyId, model);
                }
                pharmaciesList[cheapestLink.PharmacyId].Medicaments.Add(new ResponseMedicamentModel(cheapestLink.MedicamentId, medicament.Title, cheapestLink.Price, medicament.Count));
                totalPrice += cheapestLink.Price * medicament.Count;
            }

            return pharmaciesList;
        }
        
        private static Dictionary<int, ResponsePharmacyModel> GetThreePharmacies(RequestAggregateModel requestAggregateModel, 
            PharmacyMedicamentLink[] pharmacyMedicamentLinks, ref double totalPrice)
        {
            var clientMedicaments = requestAggregateModel.Medicaments;
            var clientMedicamentIds = requestAggregateModel.Medicaments.Select(e => e.Id);
            var clientMedicamentLinks = pharmacyMedicamentLinks
                .Where(e => clientMedicamentIds.Contains(e.MedicamentId)).ToArray();
            var pharmacyIds = pharmacyMedicamentLinks.Select(e => e.PharmacyId).Distinct().ToArray();
            
            var clientMedicamentLinksDictionary = new Dictionary<int, Dictionary<int, PharmacyMedicamentLink>>();

            foreach (var link in clientMedicamentLinks)
            {
                if (!clientMedicamentLinksDictionary.ContainsKey(link.MedicamentId))
                {
                    clientMedicamentLinksDictionary.Add(link.MedicamentId, new Dictionary<int, PharmacyMedicamentLink>());
                }

                if (!clientMedicamentLinksDictionary[link.MedicamentId].ContainsKey(link.PharmacyId))
                {
                    clientMedicamentLinksDictionary[link.MedicamentId].Add(link.PharmacyId, link);
                }
            }
            
            var dict = new Dictionary<RequestMedicamentModel, Pharmacy>();
            var priceDict = new Dictionary<RequestMedicamentModel, double>();
            double price = Int32.MaxValue;

            foreach (var fId1 in pharmacyIds)
            {
                foreach (var fId2 in pharmacyIds)
                {
                    foreach (var fId3 in pharmacyIds)
                    {
                        if (fId1 >= fId2 || fId2 >= fId3) continue;
                        double priceLocal = 0;
                        var dictLocal = new Dictionary<RequestMedicamentModel, Pharmacy>();
                        var priceDictLocal = new Dictionary<RequestMedicamentModel, double>();
                        var isBad = false;
                        foreach (var clientMedicament in clientMedicaments)
                        {
                            var med = clientMedicamentLinksDictionary.TryGetValue(clientMedicament.Id, out var medDict);
                            
                            if (!med)
                            {
                                isBad = true;
                                continue;
                            }
                            
                            var firstLink = medDict.TryGetValue(fId1, out var value1) ? value1 : null;
                            var secondLink = medDict.TryGetValue(fId2, out var value2) ? value2 : null;
                            var thirdLink = medDict.TryGetValue(fId3, out var value3) ? value3 : null;

                            if (firstLink == null && secondLink == null && thirdLink == null)
                            {
                                isBad = true;
                                continue;
                            }

                            var linkWithCheapestPrice = new[] {firstLink, secondLink, thirdLink}.Where(e => e != null).OrderBy(e => e.Price).First();
                            priceLocal += linkWithCheapestPrice.Price;
                            dictLocal.Add(clientMedicament, linkWithCheapestPrice.Pharmacy);
                            priceDictLocal.Add(clientMedicament, linkWithCheapestPrice.Price);
                        }
                        
                        if (isBad) continue;

                        if (price > priceLocal)
                        {
                            price = priceLocal;
                            dict = dictLocal;
                            priceDict = priceDictLocal;
                        }
                    }
                }
            }

            return GetPharmacyInternal(dict, priceDict, ref totalPrice);
        }
        
        private static Dictionary<int, ResponsePharmacyModel> GetTwoPharmacies(RequestAggregateModel requestAggregateModel, 
            PharmacyMedicamentLink[] pharmacyMedicamentLinks, ref double totalPrice)
        {
            var clientMedicaments = requestAggregateModel.Medicaments;
            var clientMedicamentIds = requestAggregateModel.Medicaments.Select(e => e.Id);
            var clientMedicamentLinks = pharmacyMedicamentLinks
                .Where(e => clientMedicamentIds.Contains(e.MedicamentId)).ToArray();
            var pharmacyIds = pharmacyMedicamentLinks.Select(e => e.PharmacyId).Distinct().ToArray();
            
            var clientMedicamentLinksDictionary = new Dictionary<int, Dictionary<int, PharmacyMedicamentLink>>();

            foreach (var link in clientMedicamentLinks)
            {
                if (!clientMedicamentLinksDictionary.ContainsKey(link.MedicamentId))
                {
                    clientMedicamentLinksDictionary.Add(link.MedicamentId, new Dictionary<int, PharmacyMedicamentLink>());
                }

                if (!clientMedicamentLinksDictionary[link.MedicamentId].ContainsKey(link.PharmacyId))
                {
                    clientMedicamentLinksDictionary[link.MedicamentId].Add(link.PharmacyId, link);
                }
            }
            
            var dict = new Dictionary<RequestMedicamentModel, Pharmacy>();
            var priceDict = new Dictionary<RequestMedicamentModel, double>();
            double price = Int32.MaxValue;

            foreach (var fId1 in pharmacyIds)
            {
                foreach (var fId2 in pharmacyIds)
                {
                    if (fId1 >= fId2) continue;
                    double priceLocal = 0;
                    var dictLocal = new Dictionary<RequestMedicamentModel, Pharmacy>();
                    var priceDictLocal = new Dictionary<RequestMedicamentModel, double>();
                    var isBad = false;
                    foreach (var clientMedicament in clientMedicaments)
                    {
                        var med = clientMedicamentLinksDictionary.TryGetValue(clientMedicament.Id, out var medDict);
                        
                        if (!med)
                        {
                            isBad = true;
                            continue;
                        }
                        
                        var firstLink = medDict.TryGetValue(fId1, out var value1) ? value1 : null;
                        var secondLink = medDict.TryGetValue(fId2, out var value2) ? value2 : null;

                        if (firstLink == null && secondLink == null)
                        {
                            isBad = true;
                            continue;
                        }

                        var linkWithCheapestPrice = new[] {firstLink, secondLink}.Where(e => e != null).OrderBy(e => e.Price).First();
                        priceLocal += linkWithCheapestPrice.Price;
                        dictLocal.Add(clientMedicament, linkWithCheapestPrice.Pharmacy);
                        priceDictLocal.Add(clientMedicament, linkWithCheapestPrice.Price);
                    }
                    
                    if (isBad) continue;

                    if (price > priceLocal)
                    {
                        price = priceLocal;
                        dict = dictLocal;
                        priceDict = priceDictLocal;
                    }
                }
            }

            return GetPharmacyInternal(dict, priceDict, ref totalPrice);
        }
        
        private static Dictionary<int, ResponsePharmacyModel> GetOnePharmacy(RequestAggregateModel requestAggregateModel, 
            PharmacyMedicamentLink[] pharmacyMedicamentLinks, ref double totalPrice)
        {
            var clientMedicaments = requestAggregateModel.Medicaments;
            var clientMedicamentIds = requestAggregateModel.Medicaments.Select(e => e.Id);
            var clientMedicamentLinks = pharmacyMedicamentLinks
                .Where(e => clientMedicamentIds.Contains(e.MedicamentId)).ToArray();
            var pharmacyIds = pharmacyMedicamentLinks.Select(e => e.PharmacyId).Distinct().ToArray();
            
            var clientMedicamentLinksDictionary = new Dictionary<int, Dictionary<int, PharmacyMedicamentLink>>();

            foreach (var link in clientMedicamentLinks)
            {
                if (!clientMedicamentLinksDictionary.ContainsKey(link.MedicamentId))
                {
                    clientMedicamentLinksDictionary.Add(link.MedicamentId, new Dictionary<int, PharmacyMedicamentLink>());
                }

                if (!clientMedicamentLinksDictionary[link.MedicamentId].ContainsKey(link.PharmacyId))
                {
                    clientMedicamentLinksDictionary[link.MedicamentId].Add(link.PharmacyId, link);
                }
            }
            
            var dict = new Dictionary<RequestMedicamentModel, Pharmacy>();
            var priceDict = new Dictionary<RequestMedicamentModel, double>();
            double price = Int32.MaxValue;
            
                foreach (var fId2 in pharmacyIds)
                {
                    double priceLocal = 0;
                    var dictLocal = new Dictionary<RequestMedicamentModel, Pharmacy>();
                    var priceDictLocal = new Dictionary<RequestMedicamentModel, double>();
                    var isBad = false;
                    foreach (var clientMedicament in clientMedicaments)
                    {
                        var med = clientMedicamentLinksDictionary.TryGetValue(clientMedicament.Id, out var medDict);
                        
                        if (!med)
                        {
                            isBad = true;
                            continue;
                        }
                        
                        var secondLink = medDict.TryGetValue(fId2, out var value2) ? value2 : null;

                        if (secondLink == null)
                        {
                            isBad = true;
                            continue;
                        }

                        var linkWithCheapestPrice = new[] {secondLink}.Where(e => e != null).OrderBy(e => e.Price).First();
                        priceLocal += linkWithCheapestPrice.Price;
                        dictLocal.Add(clientMedicament, linkWithCheapestPrice.Pharmacy);
                        priceDictLocal.Add(clientMedicament, linkWithCheapestPrice.Price);
                    }
                    
                    if (isBad) continue;

                    if (price > priceLocal)
                    {
                        price = priceLocal;
                        dict = dictLocal;
                        priceDict = priceDictLocal;
                    }
                }

                return GetPharmacyInternal(dict, priceDict, ref totalPrice);
        }

        private static Dictionary<int, ResponsePharmacyModel> GetPharmacyInternal(Dictionary<RequestMedicamentModel, Pharmacy> dict,
            Dictionary<RequestMedicamentModel, double> priceDict, ref double totalPrice)
        {
            var pharmaciesList = new Dictionary<int, ResponsePharmacyModel>();
            foreach (var pair in dict)
            {
                var id = pair.Value.Id;
                if (!pharmaciesList.ContainsKey(id))
                {
                    pharmaciesList.Add(id, new ResponsePharmacyModel(id, pair.Value.Title, pair.Value.Address, 
                        pair.Value.Latitude.Value, pair.Value.Longitude.Value, new List<ResponseMedicamentModel>()));
                }
                pharmaciesList[id].Medicaments.Add(new ResponseMedicamentModel(pair.Key.Id, pair.Key.Title, priceDict[pair.Key], pair.Key.Count));
                totalPrice += priceDict[pair.Key] * pair.Key.Count;
            }
                
            return pharmaciesList;
        }

        private PharmacyMedicamentLink[] GetData(RequestAggregateModel requestAggregateModel)
        {
            using (var context = _contextFactory.CreateContext())
            {
                var ids = requestAggregateModel.Medicaments.Select(e => e.Id).ToArray();
                return context
                    .Set<PharmacyMedicamentLink>()
                    .Where(e => ids.Contains(e.MedicamentId))
                    .Include(e => e.Pharmacy)
                    .Where(e => e.Pharmacy.Latitude != null && e.Pharmacy.Longitude != null)
                    .Include(e => e.Medicament)
                    .FilterByRange(requestAggregateModel.Settings)
                    .Where(e => !(e.Pharmacy.Latitude == 56.838011 && e.Pharmacy.Longitude == 60.597465))
                    .ToArray();
            }
        }
    }
}