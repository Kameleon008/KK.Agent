namespace KK.Agent.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AgentToolAttribute(string description) : Attribute
    {
        public string? Description { get; set; } = description;
    }

}
