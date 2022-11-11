using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Diagnostics;
using System.IO;
using O11y.Social;
using System;

namespace O11y.Social
{
    public static class WebFinger
    {
        [FunctionName("WebFinger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "webfinger")] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["resource"];

            var account = name.Replace("acct:", "");
            Activity.Current?.SetTag("account.name", account);

            var username = account.Replace("@o11y.social", "");

            if (!File.Exists(Path.Combine("profiles",$"{username}.json")))
                return new NotFoundObjectResult(new {
                    CurrentDir = Directory.GetCurrentDirectory(),
                    WorkingDir = Environment.CurrentDirectory,
                    DeployedDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
                    Username = username,
                    Account = account,
                    Message = "Account Not Found"
                });

            using var stream = new FileStream(Path.Combine("profiles",$"{username}.json"), FileMode.Open);

            var profile = JsonSerializer.Deserialize<Profile>(stream);

            log.LogInformation("C# HTTP trigger function processed a request.");

            if (name != "acct:martindotnet@o11y.social")
                return new NotFoundResult();

            return new ContentResult
            {
                Content = JsonSerializer.Serialize(profile.ToActivityPubAccount()
                    , new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                }),
                ContentType = "application/json"
            };
        }
    }
}
