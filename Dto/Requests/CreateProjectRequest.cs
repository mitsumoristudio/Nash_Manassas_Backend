using System.Text.Json.Serialization;
using Project_Manassas.Model;

namespace Project_Manassas.Dto.Requests;

public class CreateProjectRequest
{
    [JsonPropertyName("projectName")]
    public required string ProjectName { get; set; }
    
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    
    [JsonPropertyName("projectNumber")]
    public required string ProjectNumber {get; set;}
    
    [JsonPropertyName("location")]
    public required string Location { get; set; }
    
    [JsonPropertyName("contractor")]
    public string? Contractor {get; set;}
    
    [JsonPropertyName("projectEstimate")]
    public required int ProjectEstimate { get; set; }
    
    [JsonPropertyName("projectManager")]
    public required string ProjectManager { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    
}