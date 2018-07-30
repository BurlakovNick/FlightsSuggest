using System;
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

        [FunctionName("RewindSubscriberOffset")]
        public static Task<IActionResult> RewindSubscriberOffsetAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(RewindSubscriberOffsetAsync), async () =>
            {
                var subscriberId = req.Query["subscriberId"];
                var timelineName = req.Query["timelineName"];
                var date = req.Query["date"];
                var offset = DateTime.Parse(date).Ticks;

                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                await flightNotifier.RewindSubscriberOffsetAsync(subscriberId, timelineName, offset);
                return new OkObjectResult("done");
            });
        }

        [FunctionName("RewindVkOffset")]
        public static Task<IActionResult> RewindVkOffsetAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(RewindVkOffsetAsync), async () =>
            {
                var vkGroup = req.Query["vkGroup"];
                var date = req.Query["date"];
                var offset = DateTime.Parse(date).Ticks;

                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                await flightNotifier.RewindVkOffsetAsync(vkGroup, offset);

                return new OkObjectResult("done");
            });
        }

        [FunctionName("ShowUserInfo")]
        public static Task<IActionResult> ShowUserInfoAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ShowUserInfoAsync), async () =>
            {
                var subscriberId = req.Query["subscriberId"];

                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                var offsets = await flightNotifier.SelectOffsetsAsync(subscriberId);

                return new OkObjectResult(offsets);
            });
        }

        [FunctionName("ShowVkTimelineOffsets")]
        public static Task<IActionResult> ShowVkTimelineOffsetsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ShowVkTimelineOffsetsAsync), async () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                var offsets = await flightNotifier.SelectVkOffsetsAsync();

                return new OkObjectResult(offsets);
            });
        }
    }
}