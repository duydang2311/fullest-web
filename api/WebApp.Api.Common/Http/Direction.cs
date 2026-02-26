namespace WebApp.Api.Common.Http;

public readonly struct Direction
{
    public bool IsDescending { get; }
    public bool IsAscending => !IsDescending;

    public Direction()
    {
        IsDescending = false;
    }

    public Direction(bool isDescending)
    {
        IsDescending = isDescending;
    }
}
