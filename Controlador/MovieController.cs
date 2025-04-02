using CSCache.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace CSCache.Controlador
{
    public class MovieController
    {
        /*
        private static readonly HttpClient client = new HttpClient();

        public async Task<List<Feature>> GetMoviesAsync(int theatreGroupId, string filterId)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                <soap12:Body>
                    <Movies xmlns=""http://tempuri.org/CSwsInfo/Info"">
                        <TheatreGroupId>{theatreGroupId}</TheatreGroupId>
                        <FilterId>{filterId}</FilterId>
                    </Movies>
                </soap12:Body>
            </soap12:Envelope>";

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            var response = await client.PostAsync("http://cinesatlas.ddns.net/cswsinfo/info.asmx", content);
            response.EnsureSuccessStatusCode();

            var responseXml = await response.Content.ReadAsStringAsync();
            return ParseResponse(responseXml);
        }

        private List<Feature> ParseResponse(string xml)
        {
            var features = new List<Feature>();
            XDocument doc = XDocument.Parse(xml);
            var featureElements = doc.Descendants("Feature");

            foreach (var featureElement in featureElements)
            {
                var feature = new Feature
                {
                    FeatureId = (int)featureElement.Element("FeatureId"),
                    Title = (string)featureElement.Element("Title"),
                    OriginalTitle = (string)featureElement.Element("OriginalTitle"),
                    SubTitled = (string)featureElement.Element("SubTitled") == "1",
                    Dubbed = (string)featureElement.Element("Dubbed") == "1",
                    TotalRuntime = (int)featureElement.Element("TotalRuntime"),
                    Rating = (string)featureElement.Element("Rating"),
                    Genre = (string)featureElement.Element("Genre"),
                    PremierDate = DateTime.ParseExact((string)featureElement.Element("PremierDate").Value.Trim(), "yyyyMMdd", null),
                    Language = (string)featureElement.Element("Language"),
                    ShortSynopsis = (string)featureElement.Element("ShortSynopsis"),
                    TechnologyID = (int)featureElement.Element("TechnologyID"),
                    TechnologyName = (string)featureElement.Element("TechnologyName")
                };

                features.Add(feature);
            }

            return features;
        }
        */
    }
}