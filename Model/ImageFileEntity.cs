namespace Project_Manassas.Model;

public class ImageFileEntity
{
    public  int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    public byte[] Data { get; set; }
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    
    public ICollection<ProjectEntity>? Projects { get; set; }
    
}