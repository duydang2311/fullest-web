using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Hashing;
using WebApp.Api.Common.Http;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.Users.V1;

public static class CreateUser
{
    public sealed record Request(
        AuthProvider Provider,
        string Name,
        string? Password,
        string? GoogleAuthorizationCode
    );

    public sealed record Response(UserId UserId);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(a => a.Provider).IsInEnum().WithErrorCode(ErrorCodes.Invalid);
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.Empty)
                .MinimumLength(3)
                .WithErrorCode(ErrorCodes.MinLength)
                .MaximumLength(32)
                .WithErrorCode(ErrorCodes.MaxLength);
            When(
                a => a.Provider == AuthProvider.Credentials,
                () =>
                {
                    RuleFor(x => x.Password)
                        .NotEmpty()
                        .WithErrorCode(ErrorCodes.Empty)
                        .MinimumLength(8)
                        .WithErrorCode(ErrorCodes.MinLength)
                        .MaximumLength(64)
                        .WithErrorCode(ErrorCodes.MaxLength);
                }
            );
            When(
                a => a.Provider == AuthProvider.Google,
                () =>
                {
                    RuleFor(x => x.GoogleAuthorizationCode)
                        .NotEmpty()
                        .WithErrorCode(ErrorCodes.Empty);
                }
            );
        }
    }

    public static async Task<
        Results<BadRequest<ProblemDetails>, Conflict<ProblemDetails>, Created<Response>>
    > HandleAsync(
        Request request,
        AppDbContext db,
        IPasswordHasher passwordHasher,
        INumberEncoder numberEncoder,
        CancellationToken ct
    )
    {
        var conflicted = await db
            .Users.IgnoreQueryFilters()
            .AnyAsync(a => a.Name.Equals(request.Name), ct)
            .ConfigureAwait(false);
        if (conflicted)
        {
            return TypedResults.Conflict(
                Problem.FromError(nameof(request.Name), ErrorCodes.Conflict).Build()
            );
        }

        // TODO: strategy pattern if not overkill
        UserAuth auth;
        switch (request.Provider)
        {
            case AuthProvider.Credentials:
            {
                auth = new UserAuthCredentials
                {
                    Hash = passwordHasher.Hash(Guard.Against.Null(request.Password)),
                };
                break;
            }
            case AuthProvider.Google:
            {
                // TODO: Exchange GoogleAuthorizationCode for GoogleId
                auth = new UserAuthGoogle { GoogleId = "TODO" };
                break;
            }
            default:
                throw new InvalidOperationException();
        }

        var user = new User { Name = request.Name, Auths = [auth] };
        await db.AddAsync(user, ct).ConfigureAwait(false);
        try
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (UniqueConstraintException)
        {
            return TypedResults.Conflict(
                Problem.FromError(nameof(user.Name), ErrorCodes.Conflict).Build()
            );
        }
        return TypedResults.Created(
            $"/api/users/{numberEncoder.Encode(user.Id.Value)}",
            new Response(user.Id)
        );
    }
}
