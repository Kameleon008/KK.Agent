using System.ComponentModel;
using System.Text;
using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Attributes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KK.Agent.WebAPI.Agents.Tools
{
    public class OrchestratorTools(IServiceProvider provider)
    {
        private readonly AgentsFactory _agentsFactory = provider.GetRequiredService<AgentsFactory>();

        [AgentTool("List available Agents")]
        public async Task<string> list_available_agents()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var sb = new StringBuilder();
            var markdownFiles = Directory.GetFiles("./Agents", "*.md");

            foreach (var filePath in markdownFiles)
            {
                try
                {
                    string fileContent = await File.ReadAllTextAsync(filePath);
                    string yamlContent = ExtractYamlFrontMatter(fileContent);

                    if (!string.IsNullOrEmpty(yamlContent))
                    {
                        var metadata = deserializer.Deserialize<AgentMetadata>(yamlContent);

                        if (!string.IsNullOrEmpty(metadata.Name))
                        {
                            sb.AppendLine($"{metadata.Name} - {metadata.Description}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"[Error parsing file {Path.GetFileName(filePath)}: {ex.Message}]");
                }
            }

            return sb.ToString().TrimEnd();
        }

        [AgentTool("Orchestrate task to Agent")]
        public async Task<string> orchestrate_task_to_agent(
            [Description("name of available agent")] string name,
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync(name);

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }

        private string ExtractYamlFrontMatter(string fileContent)
        {
            if (!fileContent.StartsWith("---"))
                return string.Empty;

            var endIndex = fileContent.IndexOf("---", 3, StringComparison.Ordinal);

            if (endIndex == -1)
            {
                return string.Empty;
            }

            var startIndex = 3;
            var length = endIndex - startIndex;

            return fileContent.Substring(startIndex, length).Trim();
        }

        public record AgentMetadata
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}
