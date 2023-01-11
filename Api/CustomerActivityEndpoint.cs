using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SharedLibrary.SystemData;
using DataServices.Services;
using Azure;

namespace Api
{
    public class CustomerActivityEndpoint
    {
        private readonly ILogger _logger;
        private static CustomerActivityService service;

        public CustomerActivityEndpoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerActivityEndpoint>();
            service = new CustomerActivityService(Environment.GetEnvironmentVariable("ConnectionString"));
        }

        [Function("Get")]
        public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "customeractivity/{id?}")]
            HttpRequestData req, string id)
        {
            HttpResponseData response;
            var entries = service.GetAsync<SharedLibrary.CustomerActivity>(id).Result;
            var json = JsonSerializer.Serialize(entries);
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(json);
            return response;
        }

        [Function("Post")]
        public HttpResponseData CustomerPost([HttpTrigger(AuthorizationLevel.Function, "post", Route = "customeractivity")]
            HttpRequestData req)
        {
            HttpResponseData response;
            var entry = JsonSerializer.Deserialize<SharedLibrary.CustomerActivity>(req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            entry = service.AddOrUpdateAsync(entry).Result;
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(JsonSerializer.Serialize(entry));
            return response;
        }

        [Function("Delete")]
        public HttpResponseData Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "customeractivity/{id?}")]
            HttpRequestData req, string id)
        {
            HttpResponseData response;
            service.DeleteAsync(id).Wait();
            response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
