using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class FlightFunctions
    {
        [FunctionName("Notify")]
        public static Task<IActionResult> NotifyAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(NotifyAsync), async () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                await flightNotifier.NotifyAsync();
                return new OkObjectResult(flightNotifier.Sended);
            });
        }
    }
}