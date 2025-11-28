namespace Project_Manassas.Dto.Responses;

public class ProjectsResponse
{
    public IEnumerable<ProjectResponse> Items { get; init; } = Enumerable.Empty<ProjectResponse>();
  // public IEnumerable<ProjectResponse> Items { get; set; }
}