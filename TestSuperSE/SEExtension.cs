using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocsVision.Platform.StorageServer.Extensibility;
using Newtonsoft.Json;
using TestSuperSE.Model;

namespace TestSuperSE
{
    /// <summary>
    /// Класс серверного расширения
    /// </summary>
    public class SEExtension : StorageServerExtension
    {
        [ExtensionMethod]
        public string Test()
        {
            return "Hello, client!!!";
        }

        [ExtensionMethod]
        public string GetTotalPriceBackAndForth(string airportCode, DateTime flightDate, DateTime arrivalDate)
        {
            // forth ticket 
            string urlForth = "http://map.aviasales.ru/prices.json?origin_iata=LED&period=" + flightDate.Date.ToString() + ":month&direct=true&one_way=true&no_visa=false&schengen=false&need_visa=false&locale=ru";
            string resultF = getContent(urlForth);
            List<TicketData> ticketsDataF = JsonConvert.DeserializeObject<List<TicketData>>(resultF);

            decimal minPriceF = ticketsDataF
                .Where(x => x.destination == airportCode)
                .Select(p => p.value)
                .Min();

            TicketData minTicketF = ticketsDataF
                .FirstOrDefault(x =>
                x.destination == airportCode
                && x.value == minPriceF);

            // back ticket 
            string urlBack = "http://map.aviasales.ru/prices.json?origin_iata="+ airportCode +"&period=" + arrivalDate.Date.ToString() + ":month&direct=true&one_way=true&no_visa=false&schengen=false&need_visa=false&locale=ru";
            string resultB = getContent(urlForth);
            List<TicketData> ticketsDataB = JsonConvert.DeserializeObject<List<TicketData>>(resultB);

            decimal minPriceB = ticketsDataB
                .Where(x => x.destination == "LED")
                .Select(p => p.value)
                .Min();

            TicketData minTicketB = ticketsDataB
                .FirstOrDefault(x =>
                x.destination == "LED"
                && x.value == minPriceB);

            decimal totalPrice = minPriceF + minPriceB;


            return totalPrice.ToString();
        }

        [ExtensionMethod]
        private string getContent(string url)
        {
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 ....";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            response.Close();
            return output.ToString();
        }

    }
}
