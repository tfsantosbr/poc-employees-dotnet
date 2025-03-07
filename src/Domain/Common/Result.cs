using System.Collections.Generic;
using Domain.Common.Errors;

namespace Domain.Common
{
    public class Result
    {
        protected internal Result(bool isSuccess, IEnumerable<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IEnumerable<Error> Errors { get; }

        public static Result Success() => new(true, new List<Error>());

        public static Result Failure(IEnumerable<Error> errors) => new(false, errors);

        public static Result Failure(string errorCode, string errorDescription) =>
            new(false, new List<Error> { new Error(errorCode, errorDescription) });

        public static Result<T> Success<T>(T value) => new(value, true, new List<Error>());

        public static Result<T> Failure<T>(IEnumerable<Error> errors) => new(default, false, errors);

        public static Result<T> Failure<T>(string errorCode, string errorDescription) =>
            new(default, false, new List<Error> { new Error(errorCode, errorDescription) });
    }

    public class Result<T> : Result
    {
        protected internal Result(T value, bool isSuccess, IEnumerable<Error> errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
