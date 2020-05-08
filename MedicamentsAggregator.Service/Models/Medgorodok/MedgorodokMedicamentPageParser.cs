using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MedicamentsAggregator.Service.Models.Client;
using MedicamentsAggregator.Service.Models.Helpers;

namespace MedicamentsAggregator.Service.Models.Medgorodok
{
    public class MedgorodokMedicamentPageParser
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly Regex PharmacyIdRegex = new Regex(".+-(\\d+)\\.html", RegexOptions.Compiled);

        public MedgorodokMedicamentPageParser(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private const string MedgorodokBaseUrl = "https://www.medgorodok.ru";
        public async Task<MedgorodokMedicamentModel> Parse(ClientMedicamentModel clientMedicamentModel)
        {
            var html = await GetMedgorodokMedicamentPageHtml(clientMedicamentModel.Url);
            
            var parser  = new HtmlParser();
            var document = parser.ParseDocument(html);

            var tasks = document
                .QuerySelectorAll(".apothecas-addresses-list-item")
                .Select(ParsePharmacy);

            var pharmacies = (await Task
                .WhenAll(tasks))
                .Where(e => e != null)
                .ToArray();

            return new MedgorodokMedicamentModel(pharmacies);

        }

        private async Task<string> GetMedgorodokMedicamentPageHtml(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(MedgorodokBaseUrl + url);
            response.EnsureSuccessStatusCode(); // TODO - try-catch
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<MedgorodokPharmacyModel> ParsePharmacy(IElement pharmacyElement)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var (id, title) = ParseTitleAndId(pharmacyElement);
                    var address = ParseAddress(pharmacyElement);
                    var price = ParsePrice(pharmacyElement);
                    return new MedgorodokPharmacyModel(id, title, address, price);
                }
                catch (NullReturnException ex)
                {
                    //LOG;
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
                //LOG
                throw new NullReturnException();
            }

            var title = titleA.InnerHtml.Trim();

            var href = titleA.GetAttribute("href");
            if (href == null)
            {
                //LOG
                throw new NullReturnException();
            }
                
            var idIsParsed = int.TryParse(PharmacyIdRegex.Match(href).Groups[1].Value, out var id);
                
            if (!idIsParsed)
            {
                //LOG
                throw new NullReturnException();
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
                //LOG
                throw new NullReturnException();
            }

            return Math.Round(price, 1, MidpointRounding.AwayFromZero);
        }
    }
}