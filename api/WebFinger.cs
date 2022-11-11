using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Diagnostics;
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
        log.LogInformation(name);
        log.LogInformation(req.ToString());

        Activity.Current?.SetTag("account.name", name);
        log.LogInformation("C# HTTP trigger function processed a request.");

        if (name != "acct:martindotnet@o11y.social")
            return new NotFoundResult();

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(new ActivityPubAccount
            {
                Subject = "acct:Martindotnet@hachyderm.io",
                Aliases = new List<string> {
                "https://hachyderm.io/@Martindotnet",
                "https://hachyderm.io/users/Martindotnet"
            },
                Links = new List<Link> {
                new Link {
                    Relation = "http://webfinger.net/rel/profile-page",
                    Type = "text/html",
                    Href = "https://hachyderm.io/@Martindotnet"
                },
                new Link {
                    Relation = "self",
                    Type = "application/activity+json",
                    Href = "https://hachyderm.io/users/Martindotnet"
                },
                new Link {
                    Relation = "http://ostatus.org/schema/1.0/subscribe",
                    Template = "https://hachyderm.io/authorize_interaction?uri={uri}"
                }
            }
            }, new JsonSerializerOptions
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
