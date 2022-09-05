using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using ElectricityDataAPI.Models;
using HtmlAgilityPack;

namespace ElectricityDataAPI.Helper
{
    /// <summary>
    /// This class helps to seed data on model creation
    /// </summary>
    public static class ElectricitySeedingData
    {
        /// <summary>
        /// Retrieves last 2 months links to data download
        /// </summary>
        /// <returns>Uri list</returns>
        public static async Task<IEnumerable<Uri>> GetHrefs()
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(
                "https://data.gov.lt/dataset/siame-duomenu-rinkinyje-pateikiami-atsitiktinai-parinktu-1000-buitiniu-vartotoju-automatizuotos-apskaitos-elektriniu-valandiniai-duomenys");

            var hrefs = doc.DocumentNode.SelectNodes("//table[@id='resource-table']/tbody/tr//a[@href]")
                .TakeLast(2)
                .Select(node => node.GetAttributeValue("href", null))
                .Select(href => new Uri(new Uri(@"https://data.gov.lt/"), href));

            return hrefs;
        }

        /// <summary>
        /// Classifies data
        /// </summary>
        /// <param name="hrefs">links to csv files</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Grouped ElectricityReport Data</returns>
        public static async IAsyncEnumerable<ElectricityReport> ClassifyData(IEnumerable<Uri> hrefs, [EnumeratorCancellation] CancellationToken token)
        {
            foreach (var href in hrefs)
            {
                var realEstates = GetParsedElectricityData(href.AbsoluteUri, token)
                    .GroupBy(parsed => (parsed.region, parsed.houseType, parsed.objectType, parsed.objectNumber))
                    .SelectMany(grouping =>
                    {
                        var realEstate = new RealEstate
                        {
                            Region = grouping.Key.region,
                            HouseType = grouping.Key.houseType,
                            ObjectType = grouping.Key.objectType,
                            ObjectNumber = grouping.Key.objectNumber
                        };
                        return grouping
                            .Select(parsed => new ElectricityReport
                            {
                                ConsumedElectricity = parsed.consumed,
                                Time = parsed.time,
                                GeneratedElectricity = parsed.generated,
                                RealEstateObjectNumber = realEstate.ObjectNumber,
                                RealEstate = realEstate
                            });
                    }).WithCancellation(token);
                await foreach (var report in realEstates)
                    yield return report;
            }
        }

        /// <summary>
        /// Returns lines from csv with file streaming
        /// </summary>
        /// <param name="url">Link to file download</param>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Parsed Data for ElectricityReport</returns>
        static async IAsyncEnumerable<(
            string region, 
            HouseType houseType, 
            ObjectType objectType,
            int objectNumber,
            float? consumed,
            DateTime time,
            float? generated)> 
            GetParsedElectricityData(string url, [EnumeratorCancellation] CancellationToken token)
        {
            using var client = new HttpClient();
            await using var stream = await client.GetStreamAsync(url, token);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            await reader.ReadLineAsync(); // skip header

            while (await reader.ReadLineAsync() is { } line && !token.IsCancellationRequested)
            {
                var values = line.Split(',', StringSplitOptions.TrimEntries);
                if (values.Length != 0)
                    yield return (
                        region: values[0],
                        houseType: values[1] == "Namas" ? HouseType.House : HouseType.Apartment,
                        objectType: values[2] switch
                        {
                            "G" => ObjectType.ProducingConsumer,
                            "N" => ObjectType.RemoteProducingConsumer,
                            _ => ObjectType.DomesticUser
                        },
                        objectNumber: int.Parse(values[3]),
                        consumed: float.TryParse(values[4], NumberStyles.Float, CultureInfo.InvariantCulture,
                            out var consumedElectricity)
                            ? consumedElectricity
                            : null,
                        time: DateTime.ParseExact(values[5], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        generated: float.TryParse(values[4], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out var generatedElectricity)
                            ? generatedElectricity
                            : null);
            }
        }
    }
}
