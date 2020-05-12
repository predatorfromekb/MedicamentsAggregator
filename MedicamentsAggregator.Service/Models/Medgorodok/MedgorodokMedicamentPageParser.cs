using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MedicamentsAggregator.Service.DataLayer.Tables;
using MedicamentsAggregator.Service.Models.Helpers;
using MedicamentsAggregator.Service.Models.Logs;
using Vostok.Logging.Abstractions;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentPageParser
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MedgorodokLog _log;
        private static readonly Regex PharmacyIdRegex = new Regex(".+-(\\d+)\\.html", RegexOptions.Compiled);
        private const string MedgorodokBaseUrl = "https://www.medgorodok.ru";
        private const string Yekaterinburg = "Екатеринбург";

        public MedgorodokMedicamentPageParser(IHttpClientFactory httpClientFactory, MedgorodokLog log)
        {
            _httpClientFactory = httpClientFactory;
            _log = log;
        }

        public async Task<MedgorodokMedicamentModel> Parse(Medicament medicament)
        {
            var html = await GetMedgorodokMedicamentPageHtml(medicament.Url);

            var parser  = new HtmlParser();
            var document = parser.ParseDocument(html);

            var city = document
                           .QuerySelector(".apothecas-addresses-title")?
                           .InnerHtml
                           .Replace("Адреса аптек в г. ", "");

            if (city == null)
            {
                _log.Warn($"city is null: medicament - {medicament.Id}");
            }

            var tasks = document
                .QuerySelectorAll(".apothecas-addresses-list-item")
                .Select(e => ParsePharmacy(e, city));

            var pharmacies = (await Task
                .WhenAll(tasks))
                .Where(e => e != null)
                .ToArray();

            return new MedgorodokMedicamentModel(pharmacies, medicament);

        }

        private async Task<string> GetMedgorodokMedicamentPageHtml(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(MedgorodokBaseUrl + url);
            response.EnsureSuccessStatusCode(); // TODO - try-catch
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<MedgorodokPharmacyModel> ParsePharmacy(IElement pharmacyElement, string city)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var (id, title) = ParseTitleAndId(pharmacyElement);
                    var address = ParseAddress(pharmacyElement);
                    var price = ParsePrice(pharmacyElement);
                    return new MedgorodokPharmacyModel(id, title, address, price, city);
                }
                catch (NullReturnException ex)
                {
                    _log.Error(ex);
                    return null;
                }
            });
        }

        private (int Id, string Title) ParseTitleAndId(IElement pharmacyElement)
        {
            var titleA = pharmacyElement
                .Children
                .FirstOrDefault(e => e.ClassName.Contains("apothecas-addresses-list-item-name"))?
                .Children
                .FirstOrDefault(e => e.ClassName.Contains("apothecas-addresses-list-item-name-field"))?
                .Children
                .FirstOrDefault(e => e.TagName == "A");

            if (titleA == null)
            {
                throw new NullReturnException($"titleA is null: {pharmacyElement}");
            }

            var title = titleA.InnerHtml.Trim();

            var href = titleA.GetAttribute("href");
            if (href == null)
            {
                throw new NullReturnException($"href is null: {pharmacyElement}");
            }
                
            var idIsParsed = int.TryParse(PharmacyIdRegex.Match(href).Groups[1].Value, out var id);
                
            if (!idIsParsed)
            {
                throw new NullReturnException($"id cannot be parsed: {pharmacyElement}");
            }

            return (id, title);
        }

        private string ParseAddress(IElement pharmacyElement)
        {
            return pharmacyElement
                .Children
                .FirstOrDefault(e => e.ClassName.Contains("apothecas-addresses-list-item-contacts"))?
                .Children
                .FirstOrDefault(e => e.ClassName.Contains("apothecas-addresses-list-item-contacts-address"))?
                .InnerHtml
                .Trim();
        }
        
        private double ParsePrice(IElement pharmacyElement)
        {
            var priceSpanValue = pharmacyElement
                .Children
                .FirstOrDefault(e => e.ClassName.Contains("apothecas-addresses-list-item-price"))?
                .Children
                .FirstOrDefault(e => e.TagName == "SPAN")?
                .InnerHtml
                .Trim();

            var priceIsParsed = double.TryParse(priceSpanValue?
                    .Replace("₽", string.Empty)
                    .Replace("&nbsp;", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture,
                out var price);

            if (!priceIsParsed)
            {
                throw new NullReturnException($"price cannot be parsed: {pharmacyElement}");
            }

            return Math.Round(price, 1, MidpointRounding.AwayFromZero);
        }
    }
}