using Microsoft.AspNetCore.SignalR;
using ModelContextProtocol.Client;
using Nash_Manassas.Hub;
using Project_Manassas.Service;

namespace Project_Manassas.Service;

public class McpChatService
{
   private readonly McpBridgeService _mcp;
   private readonly IHubContext<ProjectHub> _hub;

   public McpChatService(McpBridgeService mcpBridgeService, IHubContext<ProjectHub> hubContext)
   {
      _mcp = mcpBridgeService;
      _hub = hubContext;
   }
   
   // Chat UI sends a message → backend → MCP
   public async Task SendChatMessageAsync(string text)
   {
      var rpc = await _mcp.SendQueryAsync("chat", new { message = text });

      // Fan out the content to the chat UI (SignalR)
      foreach (var piece in rpc.Content)
      {
         if (piece.Type == "text" && piece.Text != null)
         {
            await _hub.Clients.All.SendAsync("chatMessage", new
            {
               type = "text",
               text = piece.Text
            });
         }

         if (piece.Type == "json" && piece.Json != null)
         {
            await _hub.Clients.All.SendAsync("chatMessage", new
            {
               type = "json",
               data = piece.Json.ToString()
            });
         }
      }
   }
   
   
}