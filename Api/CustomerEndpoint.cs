using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class CustomerEndpoint
    {
        private readonly ILogger _logger;


        public CustomerEndpoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerEndpoint>();
            //Environment.GetEnvironmentVariable("ConnectionString");
        }

        [Function("Customer-Get")]
        public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "people/{rowkey?}")]
            HttpRequestData req, string rowkey)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            //response.WriteString("Welcome to Azure Functions!");

            //# For testing only
            List<SharedLibrary.Customer> customers = new();
            SharedLibrary.Customer customer = new()
            {
                PartitionKey = "MAIN",
                RowKey = rowkey,
                Name = "Anders",
                LastName = "Bagge",
                Address = "Gatan 1",
                City = "Stockholm"
            };
            customers.Add(customer);
            var json = JsonSerializer.Serialize(customer);
            response.WriteString(json);
            //#

            return response;
        }
    }
}
