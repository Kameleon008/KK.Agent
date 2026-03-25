namespace KK.Agent.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterDescriptionAttribute(string description) : Attribute
    {
        public string Description { get; } = description;
    }

}
