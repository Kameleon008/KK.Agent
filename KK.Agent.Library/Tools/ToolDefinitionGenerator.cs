using System.Reflection;
using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Tools
{
    public static class ToolDefinitionGenerator
    {
        /// <summary>
        /// Generates a list of ToolDefinitions from the given object instance
        /// </summary>
        public static List<ToolDefinition> GenerateFromObject(object instance)
        {
            var type = instance.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            return methods
                .Where(m => m.GetCustomAttributes(typeof(AgentToolAttribute), false).Any())
                .Select(method => GenerateFromMethod(method, instance))
                .ToList();
        }

        /// <summary>
        /// Generates a ToolDefinition from a single method
        /// </summary>
        public static ToolDefinition GenerateFromMethod(MethodInfo method, object? instance = null)
        {
            var toolAttribute = (AgentToolAttribute)method.GetCustomAttribute(typeof(AgentToolAttribute))!;

            var functionDef = new ToolDefinitionFunction
            {
                Name = method.Name,
                Description = toolAttribute.Description ?? method.Name,
                Strict = true,
                Parameters = new ParametersSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, PropertyDefinition>(),
                    Required = new List<string>()
                }
            };

            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var paramDescription = parameter.GetCustomAttribute<ParameterDescriptionAttribute>();
                
                // Add to properties
                functionDef.Parameters.Properties[parameter.Name!] = new PropertyDefinition
                {
                    Type = GetJsonType(parameter.ParameterType),
                    Description = paramDescription?.Description ?? parameter.Name
                };

                // If parameter is required (no default value or it's string/object)
                var hasDefaultValue = parameter.HasDefaultValue;
                if (!hasDefaultValue || !IsNullableOrPrimitiveWithDefault(parameter))
                {
                    functionDef.Parameters.Required.Add(parameter.Name!);
                }
            }

            return new ToolDefinition
            {
                Type = "function",
                Function = functionDef
            };
        }

        private static string GetJsonType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime)) return "string"; // Date as string in ISO 8601 format
            
            // Handle enumerable types as array
            if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return "array";

            // Default to string for other types
            return "string";
        }

        private static bool IsNullableOrPrimitiveWithDefault(ParameterInfo parameter)
        {
            var hasDefaultValue = parameter.HasDefaultValue;
            var isNullable = Nullable.GetUnderlyingType(parameter.ParameterType) != null || 
                           !parameter.ParameterType.IsValueType;
            
            return isNullable && hasDefaultValue;
        }
    }
}
