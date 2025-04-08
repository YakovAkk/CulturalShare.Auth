using AuthenticationProto;
using ErrorOr;
using MediatR;

namespace Service.Handlers;

public class MediatRQueries
{
    public record SearchUserByNameQuery(SearchUserRequest Request) : IRequest<ErrorOr<SearchUserResponse>>;
}
