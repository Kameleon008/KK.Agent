using System.Reflection;

namespace KK.Agent.Library.Tools
{
    public static class ToolDelegateFactory
    {
        public static Func<string, Task<string>> CreateFromMethodInfo(MethodInfo method, object instance)
        {
            return async args =>
            {
                var parameters = method.GetParameters();
                var argDict = System.Text.Json.JsonDocument.Parse(args).RootElement.EnumerateObject()
                    .ToDictionary(p => p.Name, p => p.Value.ToString());

                var parameterValues = new object?[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    if (argDict.TryGetValue(param.Name!, out var argValue) && !string.IsNullOrEmpty(argValue))
                    {
                        parameterValues[i] = Convert.ChangeType(argValue, param.ParameterType);
                    }
                    else if (param.HasDefaultValue)
                    {
                        parameterValues[i] = param.DefaultValue;
                    }
                }

                var result = method.Invoke(instance, parameterValues);

                if (result is not Task task)
                {
                    return (string?)result ?? string.Empty;
                }

                await task;

                return (string?)task.GetType().GetProperty("Result")?.GetValue(task) ?? string.Empty;
            };
        }
    }
}
