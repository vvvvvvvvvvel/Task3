using System.Text.Json.Serialization;

namespace BillingAPI.ViewModels;

public record ResponseModel
{
    public enum Status
    {
        Unspecified,
        Ok,
        Failed
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status ResponseStatus { get; set; }

    public string Comment { get; set; }
}