namespace WebApp.Api.Common.Projection;

public readonly struct Projectable(object value, string? fields)
{
    public object Value { get; } = value;
    public string? Fields { get; } = fields;
}

public static class ProjectableExtensions
{
    public static Projectable ToProjectable(this object value, string? fields)
    {
        return new Projectable(value, fields);
    }
}
