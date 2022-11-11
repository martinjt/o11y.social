using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace O11y.Social;
public class Link
{
    [JsonPropertyName("rel")]
    public string Relation { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("href")]
    public string Href { get; set; }
    
    [JsonPropertyName("template")]
    public string Template { get; set; }
}

public record ActivityPubAccount
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("aliases")]
    public List<string> Aliases { get; set; }

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; }
}