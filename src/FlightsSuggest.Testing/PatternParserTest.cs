using System;
using FlightsSuggest.Core.Infrastructure;
using NUnit.Framework;

namespace FlightsSuggest.Testing
{
    [TestFixture]
    public class PatternParserTest
    {
        private const string IntExpressionFormat = "покажи билеты за последние {int} дней";

        [TestCase("покажи билеты за последние 7 дней", 7)]
        [TestCase("покажи билеты за последние дней", null)]
        [TestCase("покажи билеты за последние 0 дней", 0)]
        [TestCase("покажи билеты", null)]
        public void TestParseExpressionWithInt(string text, int? expected)
        {
            var actual = PatternParser.ParseExpressionWithInt(IntExpressionFormat, text);
            Console.WriteLine($"Success: {actual.success}, message: {actual.message}");
            Assert.AreEqual(expected, actual.result);
        }
    }
}