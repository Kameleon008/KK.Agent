using KK.Agent.Common.Attributes;

namespace KK.Agent.Common.Tools
{
    public static class ToolGenerator
    {
        public static Dictionary<string, Func<string, Task<string>>> GenerateFromObject(object? instance)
        {
            var tools = new Dictionary<string, Func<string, Task<string>>>();

            var methods = instance.GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(AgentToolAttribute), false).Any())
                .ToDictionary(
                    method => method.Name,
                    m => ToolDelegateFactory.CreateFromMethodInfo(m, instance));

            foreach (var kvp in methods)
            {
                tools[kvp.Key] = kvp.Value;
            }

            return tools;
        }
    }
}
