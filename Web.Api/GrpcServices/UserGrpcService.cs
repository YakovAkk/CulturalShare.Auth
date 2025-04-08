using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using CulturalShare.Foundation.AspNetCore.Extensions.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using static Service.Handlers.MediatRCommands;
using static Service.Handlers.MediatRQueries;

namespace WebApi.GrpcServices;

public class UserGrpcService : AuthenticationProto.UserGrpcService.UserGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(
        ILogger<UserGrpcService> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var command = new CreateUserCommand(request);
        
        var result = await _mediator.Send(command);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return new CreateUserResponse
        {
            Id = result.Value
        };
    }

    [Authorize]
    public override async Task<Empty> AllowUser(AllowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new AllowUserCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> FollowUser(FollowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new FollowUserCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> UnfollowUser(UnfollowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new UnfollowUserCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> RestrictUser(RestrictUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new RestrictUserCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<SearchUserResponse> SearchUserByName(SearchUserRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new SearchUserByNameQuery(request));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> ToggleNotifications(ToggleNotificationsRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(context.GetHttpContext());

        var result = await _mediator.Send(new ToggleNotificationsCommand(request, userId));

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
