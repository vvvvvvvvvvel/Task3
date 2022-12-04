using System.Text.Json.Serialization;

namespace Billing.Models;

public class User
{
    public User(string name, int rating, long amount)
    {
        Name = name;
        Rating = rating;
        Amount = amount;
    }
    [JsonPropertyName("name")]
    public string Name { get; init; }
    [JsonPropertyName("rating")]
    public int Rating { get; set; }
    [JsonIgnoreAttribute]
    public long Amount { get; set; }

}