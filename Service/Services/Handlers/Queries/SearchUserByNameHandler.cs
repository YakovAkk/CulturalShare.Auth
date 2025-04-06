using AuthenticationProto;
using ErrorOr;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRQueries;

namespace Service.Services.Handlers.Queries;

public class SearchUserByNameHandler : IRequestHandler<SearchUserByNameQuery, ErrorOr<SearchUserResponse>>
{
    private readonly IUserService _userService;
    public SearchUserByNameHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<SearchUserResponse>> Handle(SearchUserByNameQuery request, CancellationToken cancellationToken) =>
        _userService.SearchUserByNameAsync(request.Request);
}
