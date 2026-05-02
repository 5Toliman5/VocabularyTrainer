using Common.Wrappers;

namespace Common.Web.Wrappers
{
    public interface IApiOperationResult : IResult
    {
        ApiErrorType ErrorType { get; }
    }

    public abstract class ApiOperationResultBase<T> : ResultBase<T>, IApiOperationResult
        where T : ApiOperationResultBase<T>, new()
    {
        public ApiErrorType ErrorType { get; protected init; }

        public static T Failure(string errorMessage, ApiErrorType errorType)
        {
            return new T { Successful = false, ErrorMessage = errorMessage, ErrorType = errorType };
        }
    }

    public class ApiOperationResult : ApiOperationResultBase<ApiOperationResult>
    {
    }

    public class ApiOperationResult<TValue> : ApiOperationResultBase<ApiOperationResult<TValue>>, IResult<TValue>
    {
        public TValue Value { get; private set; }

        public static ApiOperationResult<TValue> Success(TValue value)
        {
            var result = Success();
            result.Value = value;
            return result;
        }
    }
}
