using System.Text.Json;
using Project_Manassas.Dto.Requests;
using Project_Manassas.Dto.Responses;


namespace Project_Manassas.Service;

public class McpConnectService: IMcpConnectionService  
{
    private readonly McpBridgeService _mcpBridgeService;

    public McpConnectService(McpBridgeService mcpBridgeService)
    {
        _mcpBridgeService = mcpBridgeService;
    }

    public async Task<List<ProjectResponse>> GetAllProjectsAsync()
    {
        var rpc = await _mcpBridgeService.SendQueryAsync(" list_projects_async");

        if (rpc.Content.Count == 0 || rpc.Content.First().Json == null)
            return new List<ProjectResponse>();
        
        var jsonPayload = rpc.Content.First().Json;
        
        return JsonSerializer.Deserialize<List<ProjectResponse>>(jsonPayload.ToString())!;
        
    }
    
    public async Task<List<ProjectResponse>> FindProjectbyName(string name)
    {
        var rpc = await _mcpBridgeService.SendQueryAsync("find_project_by_name_async", new { name });
        
        if (rpc.Content.Count == 0 || rpc.Content.First().Json == null)
            return new List<ProjectResponse>();

        var jsonPayload = rpc.Content.First().Json;

        return JsonSerializer.Deserialize<List<ProjectResponse>>(jsonPayload.ToString())!;
        
    }
    
    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
    {
       var rpc = await _mcpBridgeService.SendQueryAsync("create_project_async", request);
       
       
       var jsonPayload = rpc.Content.First().Json;
       
       return JsonSerializer.Deserialize<ProjectResponse>(jsonPayload.ToString())!;
    }
    
   
}