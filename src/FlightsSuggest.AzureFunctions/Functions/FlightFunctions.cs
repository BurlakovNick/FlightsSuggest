using System.IO;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
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

        [FunctionName("ReceiveTelegramUpdate")]
        public static Task ReceiveTelegramUpdateAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            TraceWriter log,
            ExecutionContext context
        )
        {
            return Function.ExecuteAsync(log, nameof(ReceiveTelegramUpdateAsync), async () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                var flightNotifier = new FlightNotifier(configuration);
                var update = await DeserializeMessageAsync();

                await flightNotifier.ProcessTelegramUpdateAsync(update, log);

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