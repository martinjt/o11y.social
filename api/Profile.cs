using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace O11y.Social;

public class Profile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("server")]
    public string Server { get; set; }

    public ActivityPubAccount ToActivityPubAccount() =>
        new ActivityPubAccount
        {
            Subject = $"acct:{Name}@{Server}",
            Aliases = new List<string> {
                    $"https://{Server}/@{Name}",
                    $"https://{Server}/users/{Name}"
            },
            Links = new List<Link> {
                    new Link {
                        Relation = "http://webfinger.net/rel/profile-page",
                        Type = "text/html",
                        Href = $"https://{Server}/@{Name}"
                    },
                    new Link {
                        Relation = "self",
                        Type = "application/activity+json",
                        Href = $"https://{Server}/users/{Name}"
                    },
                    new Link {
                        Relation = "http://ostatus.org/schema/1.0/subscribe",
                        Template = $"https://{Server}/authorize_interaction?uri={{uri}}"
                    }
            }
        };
}
