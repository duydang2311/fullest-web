namespace WebApp.Api.Common.Http;

public static class ErrorCodes
{
    public const string Invalid = "ERR_INVALID";
    public const string Required = "ERR_REQUIRED";
    public const string MinLength = "ERR_MIN_LENGTH";
    public const string MaxLength = "ERR_MAX_LENGTH";
    public const string Conflict = "ERR_CONFLICT";
    public const string Json = "ERR_JSON";
    public const string NotFound = "ERR_NOT_FOUND";
    public const string UserNotFound = "ERR_USER_NOT_FOUND";
}
