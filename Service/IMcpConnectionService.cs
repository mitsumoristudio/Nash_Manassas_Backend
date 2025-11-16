using Project_Manassas.Dto.Requests;
using Project_Manassas.Dto.Responses;

namespace Project_Manassas.Service;

public interface IMcpConnectionService
{
    Task<List<ProjectResponse>> GetAllProjectsAsync();
    
    Task<List<ProjectResponse>> FindProjectbyName(string name);
    
    Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request);
}