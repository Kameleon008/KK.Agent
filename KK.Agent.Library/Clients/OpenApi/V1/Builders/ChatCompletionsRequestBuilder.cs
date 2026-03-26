using KK.Agent.Library.Attributes;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KK.Agent.Library.Clients.OpenApi.V1.Builders
{
    public class ChatCompletionsRequestBuilder
    {
        private readonly ChatCompletionsRequest _request = new();

        public ChatCompletionsRequestBuilder SetModel(string model)
        {
            _request.Model = model;
            return this;
        }

        public ChatCompletionsRequestBuilder SetMessages(IEnumerable<ChatMessage> messages)
        {
            _request.Messages = messages.ToList<ChatMessage>();
            return this;
        }

        public ChatCompletionsRequestBuilder AddMessage(string role, string content)
        {
            var message = new ChatMessage()
            {
                Role = role,
                Content = content
            };

            _request.Messages ??= [];
            _request.Messages.Add(message);

            return this;
        }

        public ChatCompletionsRequestBuilder SetTemperature(double temperature)
        {
            _request.Temperature = temperature;
            return this;
        }

        public ChatCompletionsRequestBuilder SetMaxTokens(int maxTokens)
        {
            _request.MaxTokens = maxTokens;
            return this;
        }

        public ChatCompletionsRequestBuilder SetStop(params string[]? stopSequences)
        {
            _request.Stop = stopSequences?.ToList();
            return this;
        }

        public ChatCompletionsRequestBuilder SetResponseFormat(string type)
        {
            _request.ResponseFormat = new ChatCompletionResponseFormat { Type = type };
            return this;
        }

        public ChatCompletionsRequestBuilder SetJsonResponseFormat(string description, object schema)
        {
            _request.ResponseFormat = new ChatCompletionResponseFormat
            {
                Type = "json_schema",
                JsonSchema = new JsonSchema
                {
                    Type = "object",
                    Schema = schema,
                    Description = description
                }
            };
            return this;
        }

        public ChatCompletionsRequestBuilder SetJsonResponseFormat<T>() where T : class, new()
        {
            var schemaDictionary = BuildJsonSchemaForType<T>();
            
            _request.ResponseFormat = new ChatCompletionResponseFormat
            {
                Type = "json_schema",
                JsonSchema = new JsonSchema
                {
                    Type = "object",
                    Schema = schemaDictionary,
                    Description = $"JSON response in the format of {typeof(T).Name}"
                }
            };
            return this;
        }

        private static Dictionary<string, object> BuildJsonSchemaForType<T>() where T : class, new()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            
            var propertiesDict = new Dictionary<string, object>();
            var requiredList = new List<string>();
            
            var schemaDict = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = propertiesDict,
                ["required"] = requiredList
            };

            foreach (var prop in properties)
            {
                var jsonPropertyName = GetJsonPropertyName(prop);
                var propSchema = BuildPropertySchema(prop);
                
                // Property is required if:
                // 1. It has [Required] attribute, OR
                // 2. It's a reference type (not nullable, not string, not value type)
                bool isRequired = prop.GetCustomAttribute<RequiredAttribute>() != null ||
                                  (!prop.PropertyType.IsValueType && 
                                   !(prop.PropertyType == typeof(string)));

                if (isRequired)
                {
                    requiredList.Add(jsonPropertyName);
                }

                propertiesDict[jsonPropertyName] = propSchema;
            }

            return schemaDict;
        }

        private static string GetJsonPropertyName(PropertyInfo property)
        {
            var jsonPropertyAttr = property.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>();
            if (jsonPropertyAttr != null && !string.IsNullOrEmpty(jsonPropertyAttr.PropertyName))
            {
                return jsonPropertyAttr.PropertyName;
            }
            
            // Default to PascalCase property name
            return property.Name;
        }

        private static Dictionary<string, object> BuildPropertySchema(PropertyInfo property)
        {
            var schema = new Dictionary<string, object>();
            
            // Get all attributes for this property
            var descriptionAttr = property.GetCustomAttribute<PropertyDescriptionAttribute>();
            var rangeAttr = property.GetCustomAttribute<RangeAttribute>();
            var stringLengthAttr = property.GetCustomAttribute<StringLengthAttribute>();
            var patternAttr = property.GetCustomAttribute<PatternAttribute>();
            var enumChoicesAttr = property.GetCustomAttribute<EnumChoicesAttribute>();
            var stringChoicesAttr = property.GetCustomAttribute<StringChoicesAttribute>();
            var exampleAttr = property.GetCustomAttribute<ExampleAttribute>();
            var requiredAttr = property.GetCustomAttribute<RequiredAttribute>();

            // Add description if available
            schema["description"] = descriptionAttr?.Description ?? property.Name;

            // Determine type and build schema based on type
            var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            
            if (propType == typeof(string))
            {
                schema["type"] = "string";
                
                if (stringLengthAttr != null)
                {
                    schema["minLength"] = stringLengthAttr.MinLength;
                    schema["maxLength"] = stringLengthAttr.MaxLength;
                }
                
                if (patternAttr != null)
                {
                    schema["pattern"] = patternAttr.Pattern;
                }

                if (stringChoicesAttr != null)
                {
                    schema["enum"] = stringChoicesAttr.Choices;
                }
            }
            else if (propType == typeof(int) || propType == typeof(long) || 
                     propType == typeof(double) || propType == typeof(float) || 
                     propType == typeof(decimal))
            {
                schema["type"] = "number";
                
                if (rangeAttr != null)
                {
                    schema["minimum"] = rangeAttr.Min;
                    schema["maximum"] = rangeAttr.Max;
                }
            }
            else if (propType == typeof(bool))
            {
                schema["type"] = "boolean";
            }
            else if (typeof(IEnumerable<>).IsAssignableFrom(propType))
            {
                schema["type"] = "array";
                var itemType = propType.GetGenericArguments().FirstOrDefault();
                if (itemType != null)
                {
                    schema["items"] = BuildItemTypeSchema(itemType);
                }
            }
            else if (propType.IsEnum)
            {
                schema["type"] = "string";
                var enumValues = Enum.GetNames(propType).ToList();
                schema["enum"] = enumValues;

                if (enumChoicesAttr != null)
                {
                    var values = Enum.GetNames(enumChoicesAttr.EnumType);
                    schema["enum"] = values.ToList();
                }
            }
            else
            {
                // Nested object - recursively build inline schema
                schema["type"] = "object";
                
                var nestedProperties = propType.GetProperties();
                var nestedPropertiesDict = new Dictionary<string, object>();
                var nestedRequiredList = new List<string>();

                foreach (var nestedProp in nestedProperties)
                {
                    var jsonPropertyName = GetJsonPropertyName(nestedProp);
                    var nestedPropSchema = BuildPropertySchema(nestedProp);
                    
                    // Property is required if:
                    // 1. It has [Required] attribute, OR
                    // 2. It's a reference type (not nullable, not string, not value type)
                    bool isNestedRequired = nestedProp.GetCustomAttribute<RequiredAttribute>() != null ||
                                           (!nestedProp.PropertyType.IsValueType && 
                                            !(nestedProp.PropertyType == typeof(string)));

                    if (isNestedRequired)
                    {
                        nestedRequiredList.Add(jsonPropertyName);
                    }

                    nestedPropertiesDict[jsonPropertyName] = nestedPropSchema;
                }

                schema["properties"] = nestedPropertiesDict;
                
                // Add required array for nested object
                if (nestedRequiredList.Count > 0)
                {
                    schema["required"] = nestedRequiredList;
                }
            }

            // Add example if available
            if (exampleAttr != null)
            {
                schema["example"] = exampleAttr.Value;
            }

            return schema;
        }

        private static Dictionary<string, object> BuildItemTypeSchema(Type itemType)
        {
            var schema = new Dictionary<string, object>();
            
            if (itemType == typeof(string))
                schema["type"] = "string";
            else if (itemType == typeof(int) || itemType == typeof(long) || 
                     itemType == typeof(double) || itemType == typeof(float))
                schema["type"] = "number";
            else if (itemType == typeof(bool))
                schema["type"] = "boolean";
            else if (itemType.IsEnum)
            {
                schema["type"] = "string";
                var enumValues = Enum.GetNames(itemType).ToList();
                schema["enum"] = enumValues;
            }
            else
                schema["type"] = "object";

            return schema;
        }

        public ChatCompletionsRequestBuilder SetStream(bool stream)
        {
            _request.Stream = stream;
            return this;
        }

        public ChatCompletionsRequestBuilder SetTools(List<ToolDefinition>? tools)
        {
            _request.Tools = tools;
            return this;
        }

        public ChatCompletionsRequest Build()
        {
            return _request;
        }

        public string BuildToString()
        {
            return JsonConvert.SerializeObject(this._request, Formatting.Indented);
        }

        public StringContent BuildToHttpContent()
        {
            var jsonStringContent = JsonConvert.SerializeObject(this._request, Formatting.Indented);
            return new StringContent(jsonStringContent, Encoding.UTF8, "application/json");
        }
    }
}
