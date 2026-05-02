using Common.Wrappers;

namespace Common.Web.Wrappers
{
    public interface IApiResult : IResult
    {
        ApiErrorType ErrorType { get; }
    }

    public abstract class ApiResultBase<T> : ResultBase<T>, IApiResult
        where T : ApiResultBase<T>, new()
    {
        public ApiErrorType ErrorType { get; protected init; }

        public static T Failure(string errorMessage, ApiErrorType errorType)
        {
            return new T { Successful = false, ErrorMessage = errorMessage, ErrorType = errorType };
        }
    }

    public class ApiResult : ApiResultBase<ApiResult>
    {
    }

    public class ApiResult<TValue> : ApiResultBase<ApiResult<TValue>>, IResult<TValue>
    {
        public TValue Value { get; private set; }

        public static ApiResult<TValue> Success(TValue value)
        {
            var result = Success();
            result.Value = value;
            return result;
        }
    }
}
