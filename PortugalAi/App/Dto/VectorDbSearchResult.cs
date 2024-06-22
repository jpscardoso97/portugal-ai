namespace App.Dto;

using System.Text.Json.Serialization;

public record VectorDbSearchResult
{   
    [JsonPropertyName("text")]
    public string? Text { get; init; }
    
    [JsonPropertyName("location")]
    public string? Location { get; init; }
}