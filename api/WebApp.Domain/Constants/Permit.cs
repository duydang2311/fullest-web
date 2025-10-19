namespace WebApp.Domain.Constants;

public static class Permit
{
    public const string CreateTask = "task:create";
    public const string ReadTask = "task:read";
    public const string UpdateTask = "task:update";
    public const string DeleteTask = "task:delete";

    public const string CreateStatus = "status:create";
    public const string ReadStatus = "status:read";
    public const string UpdateStatus = "status:update";
    public const string DeleteStatus = "status:delete";

    public const string CreateComment = "comment:create";
    public const string ReadComment = "comment:read";
    public const string UpdateComment = "comment:update";
    public const string DeleteComment = "comment:delete";
}
