using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class Function
    {
        public static IActionResult Execute(
            TraceWriter log,
            string funcName,
            Func<IActionResult> func
        )
        {
            log.Info($"Lets execute {funcName} function");
            IActionResult result;

            try
            {
                result = func();
            }
            catch (Exception exception)
            {
                log.Error($"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
                result = new ExceptionResult(exception, true);
            }

            log.Info($"Finish {funcName} function");
            return result;
        }

        public static async Task<IActionResult> ExecuteAsync(
            TraceWriter log,
            string funcName,
            Func<Task<IActionResult>> func
        )
        {
            log.Info($"Lets execute {funcName} function");
            IActionResult result;

            try
            {
                result = await func();
            }
            catch (Exception exception)
            {
                log.Error($"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
                result = new ExceptionResult(exception, true);
            }

            log.Info($"Finish {funcName} function");
            return result;
        }
    }
}