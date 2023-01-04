using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class CustomerActivityEndpoint
    {
        private readonly ILogger _logger;

        public CustomerActivityEndpoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerActivityEndpoint>();
        }

        [Function("CustomerActivity-Get")]
        public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "customeractivity")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            //# For testing only
            List<SharedLibrary.CustomerActivity> activities = new();
            SharedLibrary.CustomerActivity activity;

            //#

            return response;
        }
    }
}
