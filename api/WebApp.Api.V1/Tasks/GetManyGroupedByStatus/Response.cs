using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;

namespace WebApp.Api.V1.Tasks.GetManyGroupedByStatus;

public sealed record Response(KeysetList<Projectable> List, int TotalCount);
