using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class Function
    {
        public static IActionResult Execute(
            ILogger log,
            string funcName,
            Func<IActionResult> func
        )
        {
            log.LogInformation($"Lets execute {funcName} function");
            IActionResult result;

            try
            {
                result = func();
            }
            catch (Exception exception)
            {
                log.LogError($"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
                result = new ExceptionResult(exception, true);
            }

            log.LogInformation($"Finish {funcName} function");
            return result;
        }

        public static async Task<IActionResult> ExecuteAsync(
            ILogger log,
            string funcName,
            Func<Task<IActionResult>> func
        )
        {
            log.LogInformation($"Lets execute {funcName} function");
            IActionResult result;

            try
            {
                result = await func();
            }
            catch (Exception exception)
            {
                log.LogError($"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
                result = new ExceptionResult(exception, true);
            }

            log.LogInformation($"Finish {funcName} function");
            return result;
        }

        public static async Task ExecuteTimerAsync(
            ILogger log,
            string funcName,
            Func<Task> func
        )
        {
            log.LogInformation($"Lets execute {funcName} function");

            try
            {
                await func();
            }
            catch (Exception exception)
            {
                log.LogError($"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");
            }

            log.LogInformation($"Finish {funcName} function");
        }
    }
}