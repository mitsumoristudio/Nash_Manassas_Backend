using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project_Manassas.Model;

public class ProjectEntity
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }
    
    [MaxLength(100)]
    [JsonPropertyName("projectName")]
    public required string ProjectName { get; set; }
    
    [MaxLength(200)]
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    
    [MaxLength(100)]
    [JsonPropertyName("projectNumber")]
    public required string ProjectNumber { get; set; }
    
    [MaxLength(50)]
    [JsonPropertyName("location")]
    public required string Location { get; set; }
    
    [MaxLength(50)]
    [JsonPropertyName("contractor")]
    public string? Contractor { get; set; }
    
    [MaxLength(50)] 
    [JsonPropertyName("projectEstimate")]
    public decimal ProjectEstimate { get; set; }
    
    [MaxLength(50)]
    [JsonPropertyName("projectManager")]
    public required string ProjectManager { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("userId")]
    public Guid? UserId { get; set; }
    
    public UserEntity? User { get; set; }
    
}