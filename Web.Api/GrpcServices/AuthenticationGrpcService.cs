using AuthenticationBackProto;
using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using CulturalShare.Foundation.AspNetCore.Extensions.Helpers;
using CulturalShare.Foundation.Authorization.AuthRoles;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using static Service.Handlers.MediatRCommands;

namespace WebApi.GrpcServices;

public class AuthenticationGrpcService : AuthenticationProto.AuthenticationGrpcService.AuthenticationGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthenticationGrpcService> _logger;

    public AuthenticationGrpcService(
        ILogger<AuthenticationGrpcService> log,
        IMediator mediator)
    {
        _logger = log;
        _mediator = mediator;
    }

    [Authorize(Roles = UserRoles.AllRoles)]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new RefreshTokenCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
