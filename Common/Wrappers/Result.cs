namespace Common.Wrappers
{
    public interface IResult
    {
        bool Successful { get; }
        string ErrorMessage { get; }
        ResultErrorKind ErrorKind { get; }
    }

    public abstract class ResultBase<T> : IResult
        where T : ResultBase<T>, new()
    {
        public bool Successful { get; protected init; }
        public string ErrorMessage { get; protected init; } = string.Empty;
        public ResultErrorKind ErrorKind { get; protected init; }

        public static T Success()
        {
            return new T { Successful = true };
        }

        public static T Failure(string errorMessage, ResultErrorKind kind = ResultErrorKind.Unknown)
        {
            return new T { Successful = false, ErrorMessage = errorMessage, ErrorKind = kind };
        }

        public static T Failure(IResult source)
        {
            return new T { Successful = false, ErrorMessage = source.ErrorMessage, ErrorKind = source.ErrorKind };
        }
    }

    public class Result : ResultBase<Result>
    {
    }

    public interface IResult<T> : IResult
    {
        T Value { get; }
    }

    public class Result<T> : ResultBase<Result<T>>, IResult<T>
    {
        public T Value { get; private set; } = default!;

        public static Result<T> Success(T value)
        {
            var result = Success();
            result.Value = value;
            return result;
        }
    }
}
