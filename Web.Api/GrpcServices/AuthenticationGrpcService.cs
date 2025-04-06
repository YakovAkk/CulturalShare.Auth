using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using CulturalShare.Foundation.AspNetCore.Extensions.Helpers;
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

    public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(SignIn)} request. Email = {request.Email}");

        var accessTokenResult = await _mediator.Send(new SignInCommand(request));

        accessTokenResult.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return accessTokenResult.Value;
    }

    [Authorize]
    public override async Task<Empty> SignOut(SignOutRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var accessTokenResult = await _mediator.Send(new SignOutCommand(userId));

        accessTokenResult.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return accessTokenResult.Value;
    }

    public override async Task<ServiceTokenResponse> GetServiceToken(ServiceTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetServiceToken)} request. ServiceId = {request.ServiceId}");

        var result = await _mediator.Send(new GetServiceTokenCommand(request));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new RefreshTokenCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
