using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApp.Api.Common.Codecs;
using WebApp.Domain.Commands;

namespace WebApp.Api.V1.Comments.Create;

public sealed class Endpoint(
    ICreateCommentHandler createCommentHandler,
    LinkGenerator linkGenerator,
    INumberEncoder numberEncoder
) : Endpoint<Request, Results<NotFound, Created<Response>>>
{
    public override void Configure()
    {
        Post("comments");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Created<Response>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var comment = await createCommentHandler.HandleAsync(
            new CreateComment(req.TaskId, req.CallerId)
            {
                ContentJson = req.ContentJson,
                ContentText = req.ContentText,
            },
            ct
        );

        var url = linkGenerator.GetUriByName(
            HttpContext,
            IEndpoint.GetName<GetOne.Endpoint>(),
            new { CommentId = numberEncoder.Encode(comment.Id.Value) }
        );
        return TypedResults.Created(url, new Response(comment.Id));
    }
}
