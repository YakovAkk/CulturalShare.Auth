using AuthenticationBackProto;
using CulturalShare.Common.Helper.Extensions;
using CulturalShare.Foundation.Authorization.AuthRoles;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using static Service.Handlers.MediatRCommands;

namespace WebApi.GrpcServices;

public class AuthenticationBackGrpcService : AuthenticationBackProto.AuthenticationBackGrpcService.AuthenticationBackGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthenticationGrpcService> _logger;

    public AuthenticationBackGrpcService(
        ILogger<AuthenticationGrpcService> log,
        IMediator mediator)
    {
        _logger = log;
        _mediator = mediator;
    }

    public override async Task<ServiceTokenResponse> GetServiceToken(ServiceTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetServiceToken)} request. ServiceId = {request.ServiceId}");

        var result = await _mediator.Send(new GetServiceTokenCommand(request));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize(Roles = ServiceRoles.UserService)]
    public override async Task<Empty> RevokeUserToken(RevokeUserTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(RevokeUserToken)} request. ServiceId = {request.UserId}");

        var result = await _mediator.Send(new RevokeUserTokenCommand(request));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize(Roles = ServiceRoles.UserService)]
    public override async Task<UserTokenResponse> GetUserToken(UserTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetUserToken)} request. ServiceId = {request.UserId}");

        var result = await _mediator.Send(new GetUserTokenCommand(request));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
