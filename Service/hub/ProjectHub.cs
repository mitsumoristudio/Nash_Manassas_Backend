using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Nash_Manassas.Hub
{
    public class ProjectHub : Microsoft.AspNetCore.SignalR.Hub
    {
        /// <summary>
        /// Called by React chat to send a user message.
        /// Backend then forwards to MCP Chat Service.
        /// </summary>
        public async Task SendMessage(string user, string message)
        {
            // Broadcast to all clients (UI echo)
            await Clients.All.SendAsync("userMessage", new
            {
                user,
                text = message,
                timestamp = DateTime.UtcNow
            });

            // Instead of calling MCP here directly,
            // use a backend service injected outside the Hub.
            //
            // Example:
            // await _mcpChatService.SendChatMessageAsync(message);
        }

        /// <summary>
        /// Helper for backend to push text messages (from MCP).
        /// </summary>
        public async Task BroadcastChatMessage(object payload)
        {
            await Clients.All.SendAsync("chatMessage", payload);
        }

        /// <summary>
        /// Helper for backend to push streaming token chunks.
        /// </summary>
        public async Task BroadcastChatToken(string token)
        {
            await Clients.All.SendAsync("chatToken", token);
        }

        /// <summary>
        /// Helper for backend to push project list results.
        /// </summary>
        public async Task BroadcastProjectList(object data)
        {
            await Clients.All.SendAsync("listProjects", data);
        }

        /// <summary>
        /// Helper for backend to push “find project” results.
        /// </summary>
        public async Task BroadcastFindProject(object data)
        {
            await Clients.All.SendAsync("findProject", data);
        }

        /// <summary>
        /// Helper for backend to push create project results.
        /// </summary>
        public async Task BroadcastCreateProject(object data)
        {
            await Clients.All.SendAsync("createProject", data);
        }
    }
}
