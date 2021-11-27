using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsVision.Platform.StorageServer.Extensibility;

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
            return string.Format("{0}\n{1}\n{2}", airportCode, flightDate+"", arrivalDate+"");
        }

    }
}
