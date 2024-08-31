using Newtonsoft.Json;

namespace SmartKeyCaddy.Models;

public class EmailMessage
{
    [JsonProperty("cc")]
    public List<Cc> Cc { get; set; }

    [JsonProperty("schedule")]
    public int Schedule { get; set; }

    [JsonProperty("bcc")]
    public List<Bcc> Bcc { get; set; }

    [JsonProperty("attachments")]
    public List<Attachment> Attachments { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; }

    [JsonProperty("from")]
    public From From { get; set; }

    [JsonProperty("to")]
    public List<To> To { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }
}
public class Attachment
{
    [JsonProperty("disposition")]
    public string Disposition { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("content_id")]
    public string ContentId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}

public class Bcc
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}

public class Cc
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}

public class From
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email_address_id")]
    public int EmailAddressId { get; set; }
}

public class To
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}
