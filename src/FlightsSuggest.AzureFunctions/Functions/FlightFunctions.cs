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
            return InternalNotifyAsync(log, context);
        }

        [FunctionName("NotifyByTimer")]
        public static Task NotifyByTimerAsync(
            //UTC, every day at 5:00
            //[TimerTrigger("0 */5 * * * *", RunOnStartup = false)]TimerInfo myTimer,
            
            //Every hour - temp
            [TimerTrigger("0 0 * * * *", RunOnStartup = false)]TimerInfo myTimer,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return InternalNotifyAsync(log, context);
        }

        public static Task<IActionResult> InternalNotifyAsync(TraceWriter log, ExecutionContext context)
        {
            return Function.ExecuteAsync(log, nameof(NotifyAsync), async () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                await flightNotifier.NotifyAsync();

                var sended = flightNotifier.Sended;
                log.Info($"Sended {sended.Length} notifications");

                return new OkObjectResult(sended);
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