using System.Diagnostics.CodeAnalysis;

namespace WebApp.Domain.Entities;

public sealed record Namespace
{
    public NamespaceId Id { get; init; }
    public NamespaceKind Kind { get; init; }
    public UserId? UserId { get; init; }
    public User? User { get; init; }
    public ICollection<Project> Projects { get; init; } = null!;

    [MemberNotNullWhen(true, nameof(UserId), nameof(User))]
    public bool IsUserNamespace => Kind == NamespaceKind.User;
}
