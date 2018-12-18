using System;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Microsoft.Extensions.DependencyInjection;
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
            flightNotifier = Container.Container.Build().GetService<FlightNotifier>();
        }

        [Test, Ignore("Только для ручного уведомления ВСЕХ пользователей")]
        public async Task TestNotify()
        {
            await flightNotifier.NotifyAsync();
            Console.WriteLine(JsonConvert.SerializeObject(flightNotifier.Sended, Formatting.Indented));
        }

        [Test, Ignore("Только для ручного отмотки времени")]
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
