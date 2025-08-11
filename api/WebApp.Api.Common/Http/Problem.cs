using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.Common.Http;

public sealed class Problem
{
    private string? type;
    private int? statusCode;
    private string? title;
    private string? detail;
    private string? instance;
    private List<string>? codes;
    private Dictionary<string, List<string>>? errors;

    private List<string> Codes => codes ??= [];
    private Dictionary<string, List<string>> Errors =>
        errors ??= new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    public static Problem FromDetail(string detail)
    {
        return new Problem { detail = detail };
    }

    public static Problem FromCodes(params List<string> codes)
    {
        return new Problem { codes = codes };
    }

    public static Problem FromError(string field, string code)
    {
        return new Problem
        {
            errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    field,
                    new List<string> { code }
                },
            },
        };
    }

    public Problem Type(string type)
    {
        this.type = type;
        return this;
    }

    public Problem StatusCode(int statusCode)
    {
        this.statusCode = statusCode;
        return this;
    }

    public Problem Title(string title)
    {
        this.title = title;
        return this;
    }

    public Problem Instance(string instance)
    {
        this.instance = instance;
        return this;
    }

    public Problem Code(string code)
    {
        Codes.Add(code);
        return this;
    }

    public Problem Error(string key, string error)
    {
        if (errors is null)
        {
            errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { key, [error] },
            };
            return this;
        }
        if (!errors.TryGetValue(key, out var errorList))
        {
            errors[key] = [error];
        }
        else
        {
            errorList.Add(error);
        }
        return this;
    }

    public ProblemDetails Build()
    {
        return new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = instance,
            Extensions = BuildExtensions(codes, errors),
        };
    }

    private static Dictionary<string, object?> BuildExtensions(
        List<string>? codes,
        Dictionary<string, List<string>>? errors
    )
    {
        var extensions = new Dictionary<string, object?>(StringComparer.Ordinal);

        if (codes != null && codes.Count > 0)
        {
            extensions["codes"] = codes;
        }

        if (errors != null && errors.Count > 0)
        {
            extensions["errors"] = errors;
        }

        return extensions;
    }
}
