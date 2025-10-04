namespace WebApp.Domain.Entities;

public sealed record Tag
{
    public TagId Id { get; init; }
    public string Value { get; init; } = null!;
    public ICollection<Project> Projects { get; init; } = null!;
}
