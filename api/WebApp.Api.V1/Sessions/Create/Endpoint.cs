using System.Security.Cryptography;
using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Slugify;
using WebApp.Api.Common.Http;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Sessions.Create;

public sealed class Endpoint(AppDbContext db, ISlugHelper slugHelper)
    : Endpoint<Request, Results<BadRequest<Problem>, NotFound<Problem>, Ok<Response>>>
{
    public override void Configure()
    {
        Post("sessions");
        Version(1);
        AllowAnonymous();
    }

    public override Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Ok<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.Provider);

        return req.Provider switch
        {
            AuthProvider.Google => CreateGoogleSessionAsync(
                Guard.Against.NullOrEmpty(req.GoogleIdToken),
                ct
            ),
            AuthProvider.Credentials => throw new NotImplementedException(),
            _ => throw new InvalidProgramException($"Unknown auth provider: {req.Provider}"),
        };
    }

    private async Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Ok<Response>>
    > CreateGoogleSessionAsync(string googleIdToken, CancellationToken ct)
    {
        GoogleJsonWebSignature.Payload? payload;
        try
        {
            ct.ThrowIfCancellationRequested();
            payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken);
            ct.ThrowIfCancellationRequested();
        }
        catch (InvalidJwtException)
        {
            return TypedResults.BadRequest(
                Problem.FromError(nameof(googleIdToken), ErrorCodes.Invalid)
            );
        }

        if (!payload.EmailVerified)
        {
            return TypedResults.BadRequest(Problem.FromError("ERR_EMAIL_UNVERIFIED"));
        }

        var userId = await db
            .UserAuths.OfType<UserAuthGoogle>()
            .Where(a => a.GoogleId.Equals(payload.Subject))
            .Select(a => (UserId?)a.UserId)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (!userId.HasValue)
        {
            return TypedResults.NotFound(
                Problem.FromError(nameof(Request.GoogleIdToken), ErrorCodes.NotFound)
            );
        }

        var session = new UserSession
        {
            UserId = userId.Value,
            Token = RandomNumberGenerator.GetBytes(32),
        };

        await db.AddAsync(session, ct).ConfigureAwait(false);
        try
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (ReferenceConstraintException e)
        {
            if (
                e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(UserSession.UserId), StringComparison.Ordinal)
                )
            )
            {
                return TypedResults.NotFound(Problem.FromError(ErrorCodes.UserNotFound));
            }
            throw;
        }

        return TypedResults.Ok(new Response { Token = WebEncoders.Base64UrlEncode(session.Token) });
    }

    private async Task<UserId> CreateGoogleUserAsync(
        GoogleJsonWebSignature.Payload payload,
        CancellationToken ct
    )
    {
        var baseName = GenerateUserName(payload);
        var user = new User
        {
            Name = baseName,
            Auths = [new UserAuthGoogle { GoogleId = payload.Subject }],
        };

        var retries = 0;
        while (true)
        {
            await db.Users.AddAsync(user, ct).ConfigureAwait(false);
            try
            {
                await db.SaveChangesAsync(ct).ConfigureAwait(false);
                break;
            }
            catch (UniqueConstraintException)
            {
                db.Entry(user).State = EntityState.Detached;
                user = user with
                {
                    Name = baseName + '-' + RandomNumberGenerator.GetHexString(12, true),
                };
                if (++retries > 5)
                {
                    throw new InvalidOperationException(
                        "Failed to create a unique user name after multiple attempts"
                    );
                }
            }
        }

        return user.Id;
    }

    private string GenerateUserName(GoogleJsonWebSignature.Payload payload)
    {
        if (!string.IsNullOrEmpty(payload.Name))
        {
            return slugHelper.GenerateSlug(payload.Name);
        }

        var atIndex = payload.Email.IndexOf('@');
        var part = atIndex == -1 ? payload.Email : payload.Email[..atIndex];
        return slugHelper.GenerateSlug(part);
    }
}
