using System;
using System.Net;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Dto;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace FlightsSuggest.Admin.Infrastructure
{
    public class FlightsFunctions : IFlightsFunctions
    {
        private readonly IFlightsConfiguration flightsConfiguration;
        private readonly RestClient client;

        public FlightsFunctions(
            IFlightsConfiguration flightsConfiguration
            )
        {
            this.flightsConfiguration = flightsConfiguration;
            this.client = new RestClient(flightsConfiguration.AzureFunctionsUrl);

            //note: https://bytefish.de/blog/restsharp_custom_json_serializer/
            client.AddHandler("application/json", Serializer.Instance);
            client.AddHandler("text/json", Serializer.Instance);
            client.AddHandler("text/x-json", Serializer.Instance);
            client.AddHandler("text/javascript", Serializer.Instance);
            client.AddHandler("*+json", Serializer.Instance);
        }

        public Task<SubscriberDto[]> SelectAsync()
        {
            var request = new RestRequest(flightsConfiguration.ShowSubscribersUrl);
            return ExecuteAsync<SubscriberDto[]>(request);
        }

        private async Task<T> ExecuteAsync<T>(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = Serializer.Instance;

            var response = await client.ExecuteTaskAsync<T>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Status code is {response.StatusCode}");
            }

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new InvalidOperationException($"Response status is {response.ResponseStatus}", response.ErrorException);
            }

            return response.Data;
        }

        private class Serializer : ISerializer, IDeserializer
        {
            private static readonly Serializer SerializerSingleton = new Serializer();
            public static readonly Serializer Instance = SerializerSingleton;

            private Serializer()
            {
                ContentType = "application/json";
            }

            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj, Formatting.None);
            }

            public T Deserialize<T>(IRestResponse response)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }

            public string ContentType { get; set; }

            public string RootElement { get; set; }
            public string Namespace { get; set; }
            public string DateFormat { get; set; }
        }
    }
}