using System.Collections.Generic;

namespace Domain.Common
{
    public class Result
    {
        protected internal Result(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IEnumerable<string> Errors { get; }

        public static Result Success() => new(true, new List<string>());
        
        public static Result Failure(IEnumerable<string> errors) => new(false, errors);
        
        public static Result Failure(string error) => new(false, new List<string> { error });
        
        public static Result<T> Success<T>(T value) => new(value, true, new List<string>());
        
        public static Result<T> Failure<T>(IEnumerable<string> errors) => new(default, false, errors);
        
        public static Result<T> Failure<T>(string error) => new(default, false, new List<string> { error });
    }

    public class Result<T> : Result
    {
        protected internal Result(T value, bool isSuccess, IEnumerable<string> errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public T Value { get; }
    }
}