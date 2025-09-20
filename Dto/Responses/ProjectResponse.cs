using Project_Manassas.Model;

namespace Project_Manassas.Dto.Responses;

public class ProjectResponse
{
    public required Guid Id { get; set; }
    
    public required string Projectname { get; set; }
    
    public required string Description { get; set; }
    
    public required string Projectnumber {get; set;}
    
    public required string Location { get; set; }
    
    public string? Contractor {get; set;}
    
    public required decimal Projectestimate { get; set; }
    
    public required string Projectmanager { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string? ImageFile { get; set; }
}