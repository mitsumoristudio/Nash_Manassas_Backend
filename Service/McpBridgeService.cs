using System.Text;
using System.Text.Json;

namespace Project_Manassas.Service;

public class McpBridgeService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public McpBridgeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<McpContentResponse> SendQueryAsync(string method, object? parameters = null)
    {
        var payload = new Dictionary<string, object?>
        {
          ["jsonrpc"] = "2.0",
           ["id"] = Guid.NewGuid().ToString(),
           ["method"] = method,
           ["params"] = parameters
        };
        
        var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

        var content = new StringContent(jsonBody,
            Encoding.UTF8, 
            "application/json");
       
        var response = await _httpClient.PostAsync("/mcp", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
       
        var result = JsonSerializer.Deserialize<McpRpcResponse<McpContentResponse>>(json, _jsonOptions);

        if (result == null) throw new InvalidOperationException("MCP server returned null response");
        
        if (result.Error != null)
            throw new Exception($"MCP Error: {result.Error.Message}");

        if (result.Result == null)
            throw new Exception("MCP response missing result");

        
        Console.WriteLine("POST " + _httpClient.BaseAddress + "/mcp");

        return result.Result;
    }
    
    // DTO
    public class McpRpcResponse<T>
    {
        public string Jsonrpc { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public T? Result { get; set; }
        
        public McpError? Error { get; set; }
    }
    
    public class McpContentResponse
    {
        public List<McpContent> Content { get; set; } = new();
        public object? Meta { get; set; }
    }
    public class McpContent
    {
        public string Type { get; set; } = string.Empty; // text, json
        public string? Text { get; set; }
        public object? Json { get; set; }
    }

    public class McpError
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}