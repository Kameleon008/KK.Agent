namespace KK.Agent.Common.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ParameterDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

/// <summary>
/// Provides a description for a property that helps the LLM understand what value to generate
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PropertyDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

/// <summary>
/// Specifies an enum as choices for a property, limiting the LLM to these values
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnumChoicesAttribute(Type enumType) : Attribute
{
    public Type EnumType { get; } = enumType;
}

/// <summary>
/// Specifies an enum as choices for a property, limiting the LLM to these values (generic version)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnumChoicesAttribute<TEnum> : Attribute where TEnum : Enum
{
    public Type EnumType { get; } = typeof(TEnum);

    public EnumChoicesAttribute() { }
}

/// <summary>
/// Specifies a set of string choices for a property, limiting the LLM to these values
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class StringChoicesAttribute(params string[] choices) : Attribute
{
    public string[] Choices { get; } = choices;
}

/// <summary>
/// Specifies minimum and maximum values for numeric properties
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute(double min, double max) : Attribute
{
    public double Min { get; } = min;
    public double Max { get; } = max;
}

/// <summary>
/// Specifies minimum and maximum length for string properties
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class StringLengthAttribute(int minLength, int maxLength) : Attribute
{
    public int MinLength { get; } = minLength;
    public int MaxLength { get; } = maxLength;
}

/// <summary>
/// Specifies a pattern (regex) that string values must match
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PatternAttribute(string pattern, string description) : Attribute
{
    public string Pattern { get; } = pattern;
    public string Description { get; } = description;
}

/// <summary>
/// Marks a property as required in the JSON schema (even if nullable)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute;

/// <summary>
/// Specifies an example value for a property to guide the LLM
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExampleAttribute(object? value, Type? valueType = null) : Attribute
{
    public object? Value { get; } = value;
    public Type ValueType { get; } = valueType ?? value?.GetType() ?? typeof(string);
}
