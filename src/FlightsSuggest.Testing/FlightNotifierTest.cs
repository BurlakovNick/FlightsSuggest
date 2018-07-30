using System;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class FlightNotifierTest : TestBase
    {
        private FlightNotifier flightNotifier;

        [SetUp]
        public void SetUp()
        {
            flightNotifier = new FlightNotifier(Configuration);
        }

        [Test]
        public async Task TestNotify()
        {
            await flightNotifier.NotifyAsync();
            Console.WriteLine(JsonConvert.SerializeObject(flightNotifier.Sended, Formatting.Indented));
        }

        [Test]
        public async Task TestRewindVkOffsetAsync()
        {
            const string vkGroup = "vandroukiru";
            var expected = new DateTime(2018, 07, 20);
            await flightNotifier.RewindVkOffsetAsync(vkGroup, expected.Ticks);
            var offsets = await flightNotifier.SelectVkOffsetsAsync();
            var actual = offsets.First(x => x.vkGroupName == vkGroup).offset;
            Assert.AreEqual(expected, actual);
        }
    }
}
