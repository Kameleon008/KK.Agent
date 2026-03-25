namespace KK.Agent.Library.Tools.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AgentToolAttribute : Attribute
    {
        public AgentToolAttribute() { }

        /// <summary>Initializes the attribute.</summary>
        /// <param name="name">The name to use for the function.</param>
        public AgentToolAttribute(string? name) => this.Name = name;

        /// <summary>Gets the function's name.</summary>
        /// <remarks>If null, a name will based on the name of the attributed method will be used.</remarks>
        public string? Name { get; }
    }

}
