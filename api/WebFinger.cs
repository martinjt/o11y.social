using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Hosting;

namespace O11y.Social;
public class WebFinger
{
    private readonly IHostEnvironment _hostEnvironment;

    public WebFinger(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [Function("WebFinger")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "webfinger")] HttpRequestData req)
    {
        var queryString = HttpUtility.ParseQueryString(req.Url.Query);

        var name = queryString["resource"];

        var account = name?.Replace("acct:", "");
        Activity.Current?.SetTag("account.name", account);

        var username = account?.Replace("@o11y.social", "");

        var profilePath = Path.Combine(_hostEnvironment.ContentRootPath, "profiles", $"{username}.json");

        if (!File.Exists(profilePath))
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.WriteAsJsonAsync(new
            {
                Account = account ?? "Empty Account",
                Message = "couldn't find account"
            });

            return response;
        }

        using var stream = new FileStream(profilePath, FileMode.Open);

        var profile = JsonSerializer.Deserialize<Profile>(stream);
        var foundResponse = req.CreateResponse(HttpStatusCode.OK);
        foundResponse.WriteAsJsonAsync(profile.ToActivityPubAccount());
        return foundResponse;
    }
}