namespace WebApp.Api.Common.Projection;

public sealed record Projectable(object Value, string? Fields)
{
    public static Func<T, Projectable> From<T>(string? fields)
        where T : notnull
    {
        return value => new Projectable(value, fields);
    }
}

public static class ProjectableExtensions
{
    public static Projectable ToProjectable(this object value, string? fields)
    {
        return new Projectable(value, fields);
    }
}
