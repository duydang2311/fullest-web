using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Hashing;
using WebApp.Api.Common.Http;
using WebApp.Domain.Common;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Users.Create;

public sealed class Endpoint(
    AppDbContext db,
    IPasswordHasher passwordHasher,
    INumberEncoder numberEncoder,
    LinkGenerator linkGenerator
) : Endpoint<Request, Results<Conflict<Problem>, BadRequest<Problem>, Created<Response>>>
{
    public override void Configure()
    {
        Post("users");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<
        Results<Conflict<Problem>, BadRequest<Problem>, Created<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        var conflicted = await db
            .Users.IgnoreQueryFilters()
            .AnyAsync(a => a.Name.Equals(req.Name), ct)
            .ConfigureAwait(false);
        if (conflicted)
        {
            return TypedResults.Conflict(Problem.FromError(nameof(req.Name), ErrorCodes.Conflict));
        }

        // TODO: strategy pattern if not overkill
        UserAuth auth;
        switch (req.Provider)
        {
            case AuthProvider.Credentials:
            {
                auth = new UserAuthCredentials
                {
                    Hash = passwordHasher.Hash(Guard.Against.Null(req.Password)),
                };
                break;
            }
            case AuthProvider.Google:
            {
                var created = await CreateGoogleAuthAsync(Guard.Against.Null(req.GoogleIdToken))
                    .ConfigureAwait(false);
                if (created.IsFailed)
                {
                    return created.Error;
                }
                auth = created.Value;
                break;
            }
            default:
                throw new InvalidProgramException();
        }

        var user = new User { Name = req.Name, Auths = [auth] };
        var ns = new Namespace { Kind = NamespaceKind.User, User = user };
        await db.AddRangeAsync([user, ns], ct).ConfigureAwait(false);
        try
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (UniqueConstraintException)
        {
            return TypedResults.Conflict(Problem.FromError(nameof(user.Name), ErrorCodes.Conflict));
        }
        return TypedResults.Created(
            linkGenerator.GetUriByName(
                HttpContext,
                IEndpoint.GetName<GetById.Endpoint>(),
                new { UserId = numberEncoder.Encode(user.Id.Value) }
            ),
            new Response(user.Id)
        );
    }

    private static async Task<Attempt<UserAuthGoogle, BadRequest<Problem>>> CreateGoogleAuthAsync(
        string googleIdToken
    )
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken);
        }
        catch (InvalidJwtException)
        {
            return TypedResults.BadRequest(
                Problem.FromError(nameof(Request.GoogleIdToken), ErrorCodes.Invalid)
            );
        }
        return new UserAuthGoogle { GoogleId = payload.Subject };
    }
}
