using System;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class FlightNotifierTest : TestBase
    {
        [Test]
        public async Task TestNotify()
        {
            var flightNotifier = new FlightNotifier(Configuration);
            await flightNotifier.NotifyAsync();
            Console.WriteLine(JsonConvert.SerializeObject(flightNotifier.Sended, Formatting.Indented));
        }
    }
}
