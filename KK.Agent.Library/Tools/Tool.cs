namespace KK.Agent.Library.Tools
{


    public class ToolDefinition
    {
        public string Type { get; set; } = "function";

        public ToolDefinitionFunction? Function { get; set; }
    }

    public class ToolDefinitionFunction
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public ParametersSchema? Parameters { get; set; }

        public bool? Strict { get; set; }
    }

    public class ParametersSchema
    {
        public string Type { get; set; } = "object";

        public Dictionary<string, PropertyDefinition>? Properties { get; set; }

        public List<string>? Required { get; set; }

        public bool? AdditionalProperties { get; set; } = false;
    }

    public class PropertyDefinition
    {
        public string Type { get; set; } = "string";

        public string? Description { get; set; }
    }
}
