using System.Reflection;
using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Entities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AgentToolAttribute : Attribute
    {
        public string? Description { get; set; }
        
        public AgentToolAttribute() { }
        
        public AgentToolAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterDescriptionAttribute : Attribute
    {
        public string Description { get; }
        
        public ParameterDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    public static class ToolDefinitionGenerator
    {
        /// <summary>
        /// Generuje listę ToolDefinition z podanej instancji obiektu
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
        /// Generuje ToolDefinition z pojedynczej metody
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
                
                // Dodaj do properties
                functionDef.Parameters.Properties[parameter.Name!] = new PropertyDefinition
                {
                    Type = GetJsonType(parameter.ParameterType),
                    Description = paramDescription?.Description ?? parameter.Name
                };

                // Jeśli parametr jest wymagany (nie ma default value lub jest to string/object)
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
            if (type == typeof(DateTime)) return "string"; // Data jako string w formacie ISO 8601
            
            // Obsługa typów enumerables jako array
            if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return "array";

            // Domyślnie string dla pozostałych typów
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
