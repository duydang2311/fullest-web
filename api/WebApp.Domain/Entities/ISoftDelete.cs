using NodaTime;

namespace WebApp.Domain.Entities;

public interface ISoftDelete
{
    Instant? DeletedTime { get; }
}
