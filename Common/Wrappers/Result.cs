namespace Common.Wrappers
{
    public interface IResult
    {
        bool Successful { get; }
        string ErrorMessage { get; }
    }

    public abstract class ResultBase<T> : IResult
        where T: ResultBase<T>, new()
    {
        public bool Successful { get; private init; }

        public string ErrorMessage { get; private init; }

        public static T Success()
        {
            return new T { Successful = true };
        }

        public static T Failure(string errorMessage)
        {
            return new T { Successful = false, ErrorMessage = errorMessage };
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
        public T Value { get; private set; }

        public static Result<T> Success(T value)
        {
            var result = Success();
            result.Value = value;
            return result;
        }
    }
}
