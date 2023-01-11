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
    public class CustomerEndpoint
    {
        private readonly ILogger _logger;
        private static CustomerService customerService;

        public CustomerEndpoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerEndpoint>();
            customerService = new CustomerService(Environment.GetEnvironmentVariable("ConnectionString"));
        }

        [Function("Customer-Get")]
        public HttpResponseData CustomerGet([HttpTrigger(AuthorizationLevel.Function, "get", Route = "customer/{id?}")]
            HttpRequestData req, string id)
        {
            HttpResponseData response;
            var entries = customerService.GetCustomers<SharedLibrary.Customer>(id).Result;
            var json = JsonSerializer.Serialize(entries);
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(json);
            return response;
        }

        [Function("Customer-Post")]
        public HttpResponseData CustomerPost([HttpTrigger(AuthorizationLevel.Function, "post", Route = "customer")]
            HttpRequestData req)
        {
            HttpResponseData response;
            var customer = JsonSerializer.Deserialize<SharedLibrary.Customer>(req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            customer = customerService.AddOrUpdateCustomerAsync(customer).Result;
            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(JsonSerializer.Serialize(customer));
            return response;
        }

        [Function("Customer-Delete")]
        public HttpResponseData CustomerDelete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "customer/{id?}")]
            HttpRequestData req, string id)
        {
            HttpResponseData response;
            customerService.DeleteAsync(id).Wait();
            response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [Function("ClientPrincipal-Get")]
        public HttpResponseData ClientPrincipalGet(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "clientprincipal")] HttpRequestData req,
        ILogger log)
        {
            var principal = new ClientPrincipal();

            if (req.Headers.TryGetValues("x-ms-client-principal", out var header))
            {
                var data = header.ToArray()[0];
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            principal.UserRoles = principal.UserRoles?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(JsonSerializer.Serialize(principal));
            return response;
        }
    }
}
