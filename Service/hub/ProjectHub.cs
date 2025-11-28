using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Project_Manassas.Dto.Requests;
using Project_Manassas.Dto.Responses;
using Project_Manassas.Service;

namespace Nash_Manassas.Hub
{
    public class ProjectHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IMcpConnectionService _mcpService;

        public ProjectHub(IMcpConnectionService mcpConnectionService)
        {
            _mcpService = mcpConnectionService;
        }

        public async Task SendChatMessage(string user, string message)
        {
            message = message.Trim().ToLower();

            // Notify client typing started
            await SafeSendAsync("ReceiveTyping", true);

            try
            {
                if (message.Contains("list all projects") || message.Contains("show me all projects"))
                {
                    var projects = await SafeGetAllProjectsAsync();
                    var text = projects.Count > 0
                        ? string.Join("\n", projects.Select(p => $"{p.ProjectNumber} - {p.ProjectName}"))
                        : "No projects found.";

                    await SafeSendAsync("ReceiveChatMessage", new { user = "assistant", text });
                }
                else if (message.Contains("find project") || message.Contains("show project"))
                {
                    var projectName = ExtractProjectName(message);
                    if (string.IsNullOrEmpty(projectName))
                    {
                        await SafeSendAsync("ReceiveChatMessage", new
                        {
                            user = "assistant",
                            text = "Please specify the project name, e.g., 'Find project called Demo'."
                        });
                    }
                    else
                    {
                        var projects = await SafeFindProjectByNameAsync(projectName);
                        if (projects.Count == 0)
                        {
                            await SafeSendAsync("ReceiveChatMessage", new
                            {
                                user = "assistant",
                                text = $"Project '{projectName}' not found."
                            });
                        }
                        else
                        {
                            var project = projects.First();
                            await SafeSendAsync("ReceiveChatMessage", new
                            {
                                user = "assistant",
                                text = $"{project.ProjectNumber} - {project.ProjectName}"
                            });
                        }
                    }
                }
                else if (message.Contains("create project"))
                {
                    try
                    {
                        var jsonStart = message.IndexOf("{");
                        if (jsonStart == -1)
                            throw new Exception("Please provide project details as JSON.");

                        var json = message.Substring(jsonStart);
                        var request = System.Text.Json.JsonSerializer.Deserialize<CreateProjectRequest>(json);
                        var project = await SafeCreateProjectAsync(request);

                        await SafeSendAsync("ReceiveChatMessage", new
                        {
                            user = "assistant",
                            text = $"Project created: {project.ProjectNumber} - {project.ProjectName}"
                        });
                    }
                    catch (Exception ex)
                    {
                        await SafeSendAsync("ReceiveChatMessage", new
                        {
                            user = "assistant",
                            text = $"Error creating project: {ex.Message}"
                        });
                    }
                }
                else
                {
                    await SafeSendAsync("ReceiveChatMessage", new
                    {
                        user = "assistant",
                        text = "Sorry, I didn't understand that. Try 'List all projects' or 'Find project called Demo'."
                    });
                }
            }
            finally
            {
                // Notify client typing ended
                await SafeSendAsync("ReceiveTyping", false);
            }
        }

        #region Helper Methods

        private async Task SafeSendAsync(string method, object message)
        {
            try
            {
                await Clients.Caller.SendAsync(method, message);
            }
            catch
            {
                // Ignore failures to avoid breaking SignalR connection
            }
        }

        private async Task<List<ProjectResponse>> SafeGetAllProjectsAsync()
        {
            try
            {
                return await _mcpService.GetAllProjectsAsync();
            }
            catch
            {
                return new List<ProjectResponse>();
            }
        }

        private async Task<List<ProjectResponse>> SafeFindProjectByNameAsync(string name)
        {
            try
            {
                return await _mcpService.FindProjectbyName(name);
            }
            catch
            {
                return new List<ProjectResponse>();
            }
        }

        private async Task<ProjectResponse> SafeCreateProjectAsync(CreateProjectRequest request)
        {
            try
            {
                return await _mcpService.CreateProjectAsync(request);
            }
            catch
            {
                return new ProjectResponse
                {
                    ProjectName = request.ProjectName,
                    ProjectNumber = request.ProjectNumber,
                    Id = request.UserId,
                    Description = request.Description,
                    Location = request.Location,
                    ProjectEstimate = request.ProjectEstimate,
                    ProjectManager = request.ProjectManager,
                };
            }
        }

        private string? ExtractProjectName(string message)
        {
            var words = message.Split(" ");
            var idx = Array.FindIndex(words, w => w == "called" || w == "named");
            if (idx >= 0 && idx + 1 < words.Length)
                return words[idx + 1];
            return null;
        }

        #endregion
    }
}
