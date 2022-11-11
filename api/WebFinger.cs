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

namespace O11y.Social
{
    public static class WebFinger
    {
        [FunctionName("WebFinger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "webfinger")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            string name = req.Query["resource"];

            var account = name.Replace("acct:", "");
            Activity.Current?.SetTag("account.name", account);

            var username = account.Replace("@o11y.social", "");

            var profilePath = Path.Combine(context.FunctionAppDirectory, "profiles", $"{username}.json");

            if (!File.Exists(profilePath))
                return new NotFoundObjectResult(new {
                    Username = username,
                    Account = account,
                    Message = "Account Not Found"
                });

            using var stream = new FileStream(profilePath, FileMode.Open);

            var profile = JsonSerializer.Deserialize<Profile>(stream);

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
