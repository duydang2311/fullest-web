using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace WebApp.Api.Common.Projection;

public static class FieldProjector
{
    private static readonly ConcurrentDictionary<Type, TypeProjection> projectionCache = new();

    public static void Project(
        Utf8JsonWriter writer,
        object instance,
        string fields,
        JsonSerializerOptions options
    )
    {
        ProjectInternal(writer, instance, BuildFieldTreeSpan(fields), options);
    }

    private static void ProjectInternal(
        Utf8JsonWriter writer,
        object instance,
        IDictionary<string, object?> fieldTree,
        JsonSerializerOptions options
    )
    {
        if (instance is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (instance is IEnumerable enumerable)
        {
            writer.WriteStartArray();
            foreach (var element in enumerable)
            {
                if (element is null)
                {
                    continue;
                }
                ProjectInternal(writer, element, fieldTree, options);
            }
            writer.WriteEndArray();
            return;
        }

        var namingPolicy = options.PropertyNamingPolicy ?? JsonNamingPolicy.CamelCase;
        var projection = projectionCache.GetOrAdd(
            instance.GetType(),
            static a => new TypeProjection(a)
        );
        writer.WriteStartObject();
        foreach (var kvp in fieldTree)
        {
            var property = projection.Properties.GetValueOrDefault(kvp.Key);
            var fieldValue = property?.Getter(instance);

            writer.WritePropertyName(namingPolicy.ConvertName(kvp.Key));
            if (fieldValue is null)
            {
                writer.WriteNullValue();
            }
            else if (kvp.Value is not IDictionary<string, object?> innerTree)
            {
                JsonSerializer.Serialize(writer, fieldValue, options);
            }
            else
            {
                ProjectInternal(writer, fieldValue, innerTree, options);
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

            var isNullable = propertyInfo.PropertyType.IsValueType
                ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) is not null
                : IsNullableReferenceType(propertyInfo);
            var isCollection =
                propertyInfo.PropertyType.IsAssignableTo(typeof(IEnumerable))
                && propertyInfo.PropertyType != typeof(string);

            Expression bind;
            if (isCollection)
            {
                var innerType = propertyInfo.PropertyType.GetGenericArguments()[0];
                var innerParameter = Expression.Parameter(innerType);
                var memberInit = BuildMemberInitExpression(innerType, innerParameter, dict);
                var selectMethod = typeof(Enumerable)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(innerType, innerType);
                var toListMethod = typeof(Enumerable)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                    .MakeGenericMethod(innerType);
                bind = Expression.Convert(
                    Expression.Call(
                        toListMethod,
                        Expression.Call(
                            selectMethod,
                            Expression.Property(parameter, propertyInfo),
                            Expression.Lambda(memberInit, innerParameter)
                        )
                    ),
                    propertyInfo.PropertyType
                );
            }
            else
            {
                bind = BuildMemberInitExpression(
                    propertyInfo.PropertyType,
                    Expression.Property(parameter, propertyInfo),
                    dict
                );
            }

            bindings.Add(
                isNullable
                    ? Expression.Bind(
                        propertyInfo,
                        Expression.Condition(
                            Expression.Equal(
                                Expression.Property(parameter, propertyInfo),
                                Expression.Constant(null, propertyInfo.PropertyType)
                            ),
                            Expression.Constant(null, propertyInfo.PropertyType),
                            bind
                        )
                    )
                    : Expression.Bind(propertyInfo, bind)
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

        private static Func<object, object?> CompileGetterExpressionCollection(
            PropertyInfo prop,
            Dictionary<string, object?> tree
        )
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

    private static readonly NullabilityInfoContext context = new();

    private static bool IsNullableReferenceType(PropertyInfo propertyInfo)
    {
        var nullability = context.Create(propertyInfo);
        return nullability.WriteState == NullabilityState.Nullable;
    }
}
