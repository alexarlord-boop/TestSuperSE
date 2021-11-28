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

            TicketData ticketF = getMinPriceTicketOnDate(flightDate, "LED", airportCode);
            TicketData ticketB = getMinPriceTicketOnDate(arrivalDate, airportCode, "LED");

            if (ticketB == null || ticketF == null)
            {
                return "";
            }
            else
            {
                decimal total = ticketF.value + ticketB.value;
                return total + "";
            }


        }

        [ExtensionMethod]
        public TicketData getMinPriceTicketOnDate(DateTime flightDate, string origin, string destination)
        {
            string date = flightDate.Date.ToString("yyyy-MM-dd");
            string urlForth = "http://map.aviasales.ru/prices.json?origin_iata=" + origin + "&period=" + date + ":month&direct=true&one_way=true&no_visa=false&schengen=false&need_visa=false&locale=ru";
            string resultF = GetContent(urlForth);
            List<TicketData> ticketsData = JsonConvert.DeserializeObject<List<TicketData>>(resultF);
            var tickets = ticketsData
                .Where(x => x.destination == destination
                    && x.depart_date.Date.ToString("yyyy-MM-dd") == date).ToArray();

            if (tickets.Length == 0)
            {
                return null;
            }
            else
            {
                decimal minPrice = tickets.Select(t => t.value).Min();
                return tickets.FirstOrDefault(t => t.value == minPrice);
            }
        }

        [ExtensionMethod]
        public string GetContent(string url)
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
