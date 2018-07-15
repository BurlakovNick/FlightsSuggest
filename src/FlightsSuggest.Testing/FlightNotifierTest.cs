using System;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Functions;
using FlightsSuggest.Core.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class FlightNotifierTest
    {
        [Test]
        public async Task TestNotify()
        {
            var configuration = ConfigurationProvider.ProvideLocal();
            var flightNotifier = new FlightNotifier(configuration);
            await flightNotifier.NotifyAsync();
            Console.WriteLine(JsonConvert.SerializeObject(flightNotifier.Sended, Formatting.Indented));
        }
    }
}
