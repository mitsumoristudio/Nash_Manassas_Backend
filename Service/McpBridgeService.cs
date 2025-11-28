using System.Text;
using System.Text.Json;

namespace Project_Manassas.Service;

public class McpBridgeService
{
    private readonly HttpClient _httpClient;
    private string? _sessionId;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public McpBridgeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    private async Task EnsureSessionAsync()
    {
        if (!string.IsNullOrEmpty(_sessionId))
            return;

        var payload = new Dictionary<string, object?>
        {
            ["jsonrpc"] = "2.0",
            ["id"] = Guid.NewGuid().ToString(),
            ["method"] = "start_session",
            ["params"] = new { } // optional parameters if needed
        };

        var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<McpRpcResponse<McpSessionResponse>>(json, _jsonOptions);

        if (result?.Result?.SessionId == null)
            throw new Exception("Failed to start MCP session");

        _sessionId = result.Result.SessionId;
        Console.WriteLine($"MCP Session started: {_sessionId}");
    }
    
    
    
    public async Task<McpContentResponse> SendQueryAsync(string method, object? parameters = null, string? sessionId = null)
    {
        var payload = new Dictionary<string, object?>
        {
            ["jsonrpc"] = "2.0",
            ["id"] = Guid.NewGuid().ToString(),
            ["method"] = method,
            ["params"] = parameters
        };
    
        var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

        var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        
        request.Headers.Add("Mcp-Session-Id", sessionId);

        if (sessionId != null)
            request.Headers.Add("Mcp-Session-Id", sessionId);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<McpRpcResponse<McpContentResponse>>(json, _jsonOptions);

        if (result?.Error != null)
            throw new Exception($"MCP Error: {result.Error.Message}");

        return result?.Result ?? throw new InvalidOperationException("MCP response missing result");
    }
    
    // public async Task<McpContentResponse> SendQueryAsync(string method, object? parameters = null)
    // {
    //     var payload = new Dictionary<string, object?>
    //     {
    //       ["jsonrpc"] = "2.0",
    //        ["id"] = Guid.NewGuid().ToString(),
    //        ["method"] = method,
    //        ["params"] = parameters
    //     };
    //     
    //     var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
    //     
    //
    //     var content = new StringContent(jsonBody,
    //         Encoding.UTF8, 
    //         "application/json");
    //    
    //     var response = await _httpClient.PostAsync("/mcp", content);
    //     response.EnsureSuccessStatusCode();
    //
    //     var json = await response.Content.ReadAsStringAsync();
    //     
    //     Console.WriteLine("API RAW RESPONSE: " + json);
    //    
    //     var parsed = JsonSerializer.Deserialize<McpRpcResponse<McpContentResponse>>(json, _jsonOptions);
    //     
    //     Console.WriteLine("MCP Result: " + JsonSerializer.Serialize(parsed));
    //
    //     if (parsed == null) throw new InvalidOperationException("MCP server returned null response");
    //     
    //     if (parsed.Error != null)
    //         throw new Exception($"MCP Error: {parsed.Error.Message}");
    //
    //     if (parsed.Result == null)
    //         throw new Exception("MCP response missing result");
    //
    //     
    //     Console.WriteLine("POST " + _httpClient.BaseAddress + "/mcp");
    //
    //     return parsed.Result;
    // }
    
    // DTO
    public class McpRpcResponse<T>
    {
        public string Jsonrpc { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public T? Result { get; set; }
        
        public McpError? Error { get; set; }
    }

    public class McpSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
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