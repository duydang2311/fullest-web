using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace WebApp.Api.Common.Projection;

public static class FieldProjector
{
    private static readonly ConcurrentDictionary<Type, TypeProjection> projectionCache = new();

    public static IDictionary<string, object?> Project<T>(T instance, string fields)
        where T : notnull
    {
        var queue =
            new Queue<(
                IDictionary<string, object?> Tree,
                object? Instance,
                IDictionary<string, object?> Target
            )>();
        var rootTarget = new Dictionary<string, object?>(
            fields.Count(a => a == ','),
            StringComparer.Ordinal
        );
        queue.Enqueue((BuildFieldTreeSpan(fields), instance, rootTarget));
        while (queue.TryDequeue(out var item))
        {
            if (item.Instance is null)
            {
                continue;
            }
            foreach (var (field, value) in item.Tree)
            {
                var projection = projectionCache.GetOrAdd(
                    item.Instance.GetType(),
                    static a => new TypeProjection(a)
                );
                var property = projection.Properties.GetValueOrDefault(field);
                if (value is not IDictionary<string, object?> innerTree)
                {
                    if (property is not null)
                    {
                        item.Target[field] = property.Getter(item.Instance);
                    }
                    continue;
                }
                var innerTarget = new Dictionary<string, object?>(StringComparer.Ordinal);
                item.Target[field] = innerTarget;
                queue.Enqueue((innerTree, property?.Getter(item.Instance), innerTarget));
            }
        }
        return rootTarget;
    }

    public static void Project(
        Utf8JsonWriter writer,
        object instance,
        string fields,
        JsonSerializerOptions options
    )
    {
        var namingPolicy = options.PropertyNamingPolicy ?? JsonNamingPolicy.CamelCase;
        writer.WriteStartObject();
        var queue =
            new Queue<(
                IDictionary<string, object?> Tree,
                object? Instance,
                string? ObjectFieldName
            )>();
        queue.Enqueue((BuildFieldTreeSpan(fields), instance, default));
        while (queue.TryDequeue(out var item))
        {
            if (item.ObjectFieldName is not null)
            {
                writer.WritePropertyName(namingPolicy.ConvertName(item.ObjectFieldName));
                writer.WriteStartObject();
            }
            if (item.Instance is null)
            {
                continue;
            }
            foreach (var (field, value) in item.Tree)
            {
                var projection = projectionCache.GetOrAdd(
                    item.Instance.GetType(),
                    static a => new TypeProjection(a)
                );
                var property = projection.Properties.GetValueOrDefault(field);
                var fieldValue = property?.Getter(item.Instance);
                if (value is not IDictionary<string, object?> innerTree)
                {
                    if (property is not null)
                    {
                        writer.WritePropertyName(namingPolicy.ConvertName(field));
                        JsonSerializer.Serialize(writer, fieldValue, options);
                    }
                    continue;
                }
                if (fieldValue is null)
                {
                    continue;
                }
                var innerTarget = new Dictionary<string, object?>(StringComparer.Ordinal);
                queue.Enqueue((innerTree, fieldValue, field));
            }
            if (item.ObjectFieldName is not null)
            {
                writer.WriteEndObject();
            }
        }
        writer.WriteEndObject();
    }

    public static Expression<Func<T, T>> Project<T>(string fields)
    {
        var parameter = Expression.Parameter(typeof(T));
        var memberInit = BuildMemberInitExpression(
            typeof(T),
            parameter,
            BuildFieldTreeSpan(fields)
        );
        return Expression.Lambda<Func<T, T>>(memberInit, parameter);
    }

    public static Dictionary<string, object?> BuildFieldTreeSpan(string fields)
    {
        var span = fields.AsSpan();
        var tree = new Dictionary<string, object?>(span.Count(','), StringComparer.Ordinal);
        while (!span.IsEmpty)
        {
            var commaIndex = span.IndexOf(',');
            ReadOnlySpan<char> fieldSpan;
            if (commaIndex == -1)
            {
                fieldSpan = span.Trim();
                span = ReadOnlySpan<char>.Empty;
            }
            else
            {
                fieldSpan = span[..commaIndex].Trim();
                span = span[(commaIndex + 1)..];
            }

            var currentNode = tree;
            while (!fieldSpan.IsEmpty)
            {
                var periodIndex = fieldSpan.IndexOf('.');
                ReadOnlySpan<char> partSpan;

                if (periodIndex == -1)
                {
                    partSpan = fieldSpan;
                    fieldSpan = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    partSpan = fieldSpan[..periodIndex];
                    fieldSpan = fieldSpan[(periodIndex + 1)..];
                }

                var part = partSpan.ToString();
                if (currentNode.TryGetValue(part, out var node) && node is null)
                {
                    break;
                }
                if (periodIndex == -1)
                {
                    currentNode[part] = null;
                    break;
                }
                if (node is not Dictionary<string, object?> newCurrentNode)
                {
                    newCurrentNode = new Dictionary<string, object?>(StringComparer.Ordinal);
                    currentNode[part] = newCurrentNode;
                }
                currentNode = newCurrentNode;
            }
        }
        return tree;
    }

    public static MemberInitExpression BuildMemberInitExpression(
        Type type,
        Expression parameter,
        Dictionary<string, object?> fieldsTree
    )
    {
        var bindings = new List<MemberBinding>(fieldsTree.Count);
        var projection = projectionCache.GetOrAdd(type, static a => new TypeProjection(a));
        foreach (var (field, value) in fieldsTree)
        {
            if (!projection.Properties.TryGetValue(field, out var property))
            {
                continue;
            }

            var propertyInfo = property.PropertyInfo;
            if (value is not Dictionary<string, object?> dict)
            {
                bindings.Add(
                    Expression.Bind(propertyInfo, Expression.Property(parameter, propertyInfo))
                );
                continue;
            }
            bindings.Add(
                Expression.Bind(
                    propertyInfo,
                    BuildMemberInitExpression(
                        propertyInfo.PropertyType,
                        Expression.Property(parameter, propertyInfo),
                        dict
                    )
                )
            );
        }
        return Expression.MemberInit(Expression.New(type), bindings);
    }

    private sealed class TypeProjection
    {
        internal readonly ConcurrentDictionary<string, TypeProjectionProperty> Properties;

        internal TypeProjection(Type type)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Properties = new(-1, propertyInfos.Length, StringComparer.OrdinalIgnoreCase);
            foreach (var propertyInfo in propertyInfos)
            {
                Properties[propertyInfo.Name] = new TypeProjectionProperty(
                    propertyInfo,
                    CompileGetterExpression(propertyInfo)
                );
            }
        }

        private static Func<object, object?> CompileGetterExpression(PropertyInfo prop)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var castedObj = Expression.Convert(objParam, prop.DeclaringType!);
            var propertyAccess = Expression.Property(castedObj, prop);
            var castToObject = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<object, object?>>(castToObject, objParam);
            return lambda.Compile();
        }
    }

    private sealed class TypeProjectionProperty(
        PropertyInfo propertyInfo,
        Func<object, object?> getter
    )
    {
        internal readonly PropertyInfo PropertyInfo = propertyInfo;
        internal readonly Func<object, object?> Getter = getter;
    }
}
