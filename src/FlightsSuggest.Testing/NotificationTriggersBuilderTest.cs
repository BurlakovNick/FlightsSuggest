using System;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Notifications;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class NotificationTriggersBuilderTest : TestBase
    {
        [Test]
        [TestCase("([([(Ижевск)])])")]
        [TestCase("( [Греция, Салоники, Родос], [Лиссабон, Порту, Португалия], Дублин)")]
        [TestCase("([([()])])")]
        [TestCase("Екатеринбург")]
        [TestCase("[Екатеринбург, Мельбурн]")]
        public void TestParse(string text)
        {
            var (trigger, success, message) = NotificationTriggers.BuildFromText(text);
            Console.WriteLine($"Success: {success}, message: {message}");

            var actual = trigger.Serialize();
            Console.WriteLine(actual);
            Assert.AreEqual(text.WithoutWhitespaces(), actual);
        }
    }
}