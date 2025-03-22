using ErrorOr;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace Service.Temp;

public static class ExceptionHelper
{
    public static void ThrowRpcExceptionBasedOnErrorIfNeeded<T>(this ErrorOr<T> errorOrResult)
    {
        if (errorOrResult.IsError)
        {
            ThrowRpcExceptionBasedOnError(errorOrResult.Errors);
        }
    }

    public static void ThrowRpcExceptionBasedOnError(Error error)
    {
        throw CreateRpcException(error);
    }

    public static void ThrowRpcExceptionBasedOnError(IEnumerable<Error> errors)
    {
        if (errors.IsNullOrEmpty())
        {
            return;
        }

        var firstError = errors.First();
        throw CreateRpcException(firstError);
    }

    private static RpcException CreateRpcException(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCode.InvalidArgument,
            ErrorType.NotFound => StatusCode.NotFound,
            ErrorType.Unauthorized => StatusCode.Unauthenticated,
            _ => StatusCode.Unknown
        };

        return new RpcException(new Status(statusCode, error.Description));
    }
}
