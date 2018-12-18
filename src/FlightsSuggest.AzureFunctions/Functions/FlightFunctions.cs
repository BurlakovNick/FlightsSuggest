using System.IO;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Container;
using FlightsSuggest.Core.Telegram;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class FlightFunctions
    {
        [FunctionName("Notify")]
        public static Task<IActionResult> NotifyAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
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
            ILogger log,
            ExecutionContext context
        )
        {
            return InternalNotifyAsync(log, context);
        }

        public static Task<IActionResult> InternalNotifyAsync(ILogger log, ExecutionContext context)
        {
            return Function.ExecuteAsync(log, nameof(NotifyAsync), async () =>
            {
                var flightNotifier = Container.Build(context).GetFlightNotifier();
                await flightNotifier.NotifyAsync();

                var sended = flightNotifier.Sended;
                log.LogInformation($"Sended {sended.Length} notifications");

                return new OkObjectResult(sended);
            });
        }

        [FunctionName("ReceiveTelegramUpdate")]
        public static Task ReceiveTelegramUpdateAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ReceiveTelegramUpdateAsync), async () =>
            {
                var flightNotifier = Container.Build(context).GetFlightNotifier();

                var update = await DeserializeMessageAsync();
                if (update.Message == null)
                {
                    log.LogInformation("Message text is null, quiting");
                    return new OkObjectResult("ok");
                }

                var telegramUpdate = new TelegramUpdate(update);
                await flightNotifier.ProcessTelegramUpdateAsync(telegramUpdate, log);

                return new OkObjectResult("ok");
            });

            async Task<Update> DeserializeMessageAsync()
            {
                using (var streamReader = new StreamReader(req.Body))
                {
                    var bytes = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Update>(bytes);
                }
            }
        }
    }
}