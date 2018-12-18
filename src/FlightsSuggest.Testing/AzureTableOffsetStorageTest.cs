using System;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class AzureTableOffsetStorageTest : TestBase
    {
        [Test]
        public async Task TestWriteAndRead()
        {
            var id = Guid.NewGuid().ToString();
            var storage = new AzureTableOffsetStorage(Configuration);

            var actual = await storage.FindAsync(id);
            Assert.IsNull(actual);

            await storage.WriteAsync(id, 42);
            actual = await storage.FindAsync(id);
            Assert.AreEqual(42, actual);

            await storage.DeleteAsync(id);
            actual = await storage.FindAsync(id);
            Assert.IsNull(actual);
        }
    }
}