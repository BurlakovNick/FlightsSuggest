using System;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using FlightsSuggest.Core.Timelines;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class AzureTableFlightNewsStorageTest : TestBase
    {
        private Guid testRunId;

        [SetUp]
        public void SetUp()
        {
            testRunId = Guid.NewGuid();
        }

        [Test]
        public async Task TestFindLatestOffsetAsync()
        {
            var storage = new AzureTableFlightNewsStorage(Configuration);

            var source1 = GetSource("source1");
            var source2 = GetSource("source2");
            var source3 = GetSource("source3");

            var flight1 = Create(source1, new DateTime(2018, 01, 01));
            var flight2 = Create(source1, new DateTime(2018, 02, 01));
            var flight3 = Create(source1, new DateTime(2018, 03, 01));
            var flight4 = Create(source2, new DateTime(2019, 01, 01));

            await storage.WriteAsync(flight1);
            await storage.WriteAsync(flight2);
            await storage.WriteAsync(flight3);
            await storage.WriteAsync(flight4);

            var actual = await storage.FindLatestOffsetAsync(source1);
            Assert.AreEqual(new DateTime(2018, 03, 01).Ticks, actual);

            actual = await storage.FindLatestOffsetAsync(source2);
            Assert.AreEqual(new DateTime(2019, 01, 01).Ticks, actual);

            actual = await storage.FindLatestOffsetAsync(source3);
            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestSelectAsync()
        {
            var storage = new AzureTableFlightNewsStorage(Configuration);

            var source = GetSource("source");

            var flight1 = Create(source, new DateTime(2018, 01, 01));
            var flight2 = Create(source, new DateTime(2018, 02, 01));
            var flight3 = Create(source, new DateTime(2018, 03, 01));

            var anotherFlight = Create(GetSource("anotherSource"), new DateTime(2019, 01, 01));

            await storage.WriteAsync(flight1);
            await storage.WriteAsync(flight2);
            await storage.WriteAsync(flight3);
            await storage.WriteAsync(anotherFlight);

            var actual = await storage.SelectAsync(0, 5, source);
            Assert.AreEqual(3, actual.Length);

            actual = await storage.SelectAsync(flight1.Date.Ticks, 1, source);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(flight2.Id, actual[0].Id);
        }

        private FlightNews Create(string source, DateTime date)
        {
            return new FlightNews
            {
                Id = Guid.NewGuid().ToString(),
                Source = source,
                Date = date,
                Url = "url",
                Text = "text"
            };
        }

        private string GetSource(string source) => "test" + source + testRunId;
    }
}
