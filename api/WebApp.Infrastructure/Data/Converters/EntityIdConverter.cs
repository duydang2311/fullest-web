using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Converters;

public sealed class EntityIdConverter<TId, TValue> : ValueConverter<TId, TValue>
    where TId : IEntityId<TValue>, new()
{
    public EntityIdConverter()
        : base(id => id.Value, value => new TId { Value = value }) { }
}
