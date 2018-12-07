using System.IO;
using FlightsSuggest.AzureFunctions.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlightsSuggest.AzureFunctions.Functions
{
    public static class HelloFunction
    {
        [FunctionName("Hello")]
        public static IActionResult Hello(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context
            )
        {
            return Function.Execute(log, nameof(Hello), () =>
            {
                var configuration = ConfigurationProvider.Provide(context);
                string name = req.Query["name"];

                string requestBody = new StreamReader(req.Body).ReadToEnd();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                var result = new
                {
                    Name = name,
                    VkAppId = configuration.VkApplicationId,
                    VkToken = configuration.VkAccessToken
                };

                log.LogInformation("Hello is done");

                return name != null
                    ? (ActionResult) new OkObjectResult(result)
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            });
        }
    }
}
