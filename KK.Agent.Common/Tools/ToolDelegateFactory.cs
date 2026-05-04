using System.Reflection;
using Newtonsoft.Json;

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
                    return SerializeResult(result);
                }

                await task;

                var taskResult = task.GetType().GetProperty("Result")?.GetValue(task);
                return SerializeResult(taskResult);
            };
        }

        private static string SerializeResult(object? result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            // If the result is already a string, return it directly
            if (result is string str)
            {
                return str;
            }

            // Otherwise, serialize to JSON
            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}
