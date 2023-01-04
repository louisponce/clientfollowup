using System.Net;
using System.Text.Json;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SharedLibrary.SystemData;
using SharedLibrary;

namespace Api;

public class Function1
{
    private readonly ILogger _logger;
    private ClientPrincipal principal;

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

        //var binpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //var roothpath = Path.GetFullPath(Path.Combine(binpath, ".."));
        //response.WriteString(File.ReadAllText(Path.Combine(roothpath, "sample-data/weather.json")));

        response.WriteString("""
                [   
                {
                  "date": "2022-01-06",
                  "temperatureC": 1,
                  "summary": "Freezing"
                },
                {
                  "date": "2022-01-07",
                  "temperatureC": 14,
                  "summary": "Bracing"
                },
                {
                  "date": "2022-01-08",
                  "temperatureC": -13,
                  "summary": "Freezing"
                },
                {
                  "date": "2022-01-09",
                  "temperatureC": -16,
                  "summary": "Balmy"
                },
                {
                  "date": "2022-01-10",
                  "temperatureC": -2,
                  "summary": "Chilly"
                }
                ]
                """);

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

        //if (!principal.UserRoles?.Any() ?? true)
        //{
        //    return new OkObjectResult(new ClaimsPrincipal());
        //}

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(principal));
        return response;
    }
}
