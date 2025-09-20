using Project_Manassas.Model;

namespace Project_Manassas.Dto.Requests;

public class CreateProjectRequest
{
    public required string Projectname { get; set; }
    
    public required string Description { get; set; }
    
    public required string Projectnumber {get; set;}
    
    public required string Location { get; set; }
    
    public string? Contractor {get; set;}
    
    public required int Projectestimate { get; set; }
    
    public required string Projectmanager { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public IFormFile? ImageFile { get; set; }
    
    public Guid UserId { get; set; }
    
}