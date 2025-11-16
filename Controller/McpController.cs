using Microsoft.AspNetCore.Mvc;
using Project_Manassas.Service;

namespace Nash_Manassas.Controller;

[ApiController]
[Route("")]
public class McpController: ControllerBase
{
    private readonly McpBridgeService _mcpBridgeService;

    public McpController(McpBridgeService mcpBridgeService)
    {
        _mcpBridgeService = mcpBridgeService;
    }

    [HttpPost("/mcp")]
    public async Task<IActionResult> SendQueryAsync([FromBody] McpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Method))
            return BadRequest("Method Name is required");

        var response = await _mcpBridgeService.SendQueryAsync(request.Method, request.Params);
        return Ok(new { id = request.Id, result = response });
        
    }
    public class McpRequest
    {
        public string Id { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        public string Method { get; set; } = string.Empty;
        public object? Params { get; set; } // optional parameters for method
    }
    
}