using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class AdminFunctions
    {
        [FunctionName("RewindSubscriberOffset")]
        public static Task<IActionResult> RewindSubscriberOffsetAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
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
            ILogger log,
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

        [FunctionName("ShowSubscribers")]
        public static Task<IActionResult> ShowSubscribersAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ShowSubscribersAsync), async () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                var subscribers = await flightNotifier.SelectSubscribersAsync();

                return new OkObjectResult(
                    subscribers
                        .Select(x => new
                        {
                            x.Id,
                            x.SendTelegramMessages,
                            x.TelegramChatId,
                            x.TelegramUsername,
                            NotificationTrigger = x.NotificationTrigger.Serialize()
                        })
                        .ToArray()
                );
            });
        }

        [FunctionName("ShowSubscriberInfo")]
        public static Task<IActionResult> ShowSubscriberInfoAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ShowSubscriberInfoAsync), async () =>
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
            ILogger log,
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

        [FunctionName("CreateSubscriber")]
        public static Task<IActionResult> CreateSubscriberAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(CreateSubscriberAsync), async () =>
            {
                var telegramUsername = req.Query["telegramUsername"];

                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);

                await flightNotifier.CreateSubscriberAsync(telegramUsername);

                return new OkObjectResult("done");
            });
        }

        [FunctionName("SetTelegramWebhook")]
        public static Task<IActionResult> SetTelegramWebhookAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(SetTelegramWebhookAsync), async () =>
            {
                string url;
                using (var streamReader = new StreamReader(req.Body))
                {
                    url = await streamReader.ReadToEndAsync();
                }

                var configuration = ConfigurationProvider.Provide(context);
                var telegramBotClient = new TelegramBotClient(configuration.TelegramBotToken);

                await telegramBotClient.SetWebhookAsync(url);

                return new OkObjectResult("done");
            });
        }
    }
}