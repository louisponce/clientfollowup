using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Reflection;

namespace Api
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Weather")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions!");

            var binpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var roothpath = Path.GetFullPath(Path.Combine(binpath, ".."));
            response.WriteString(File.ReadAllText(Path.Combine(roothpath, "sample-data/weather.json")));
            return response;
        }
    }
}
