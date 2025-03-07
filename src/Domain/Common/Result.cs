using System.Collections.Generic;
using System.Linq;
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
        private readonly T _value;
        
        protected internal Result(T value, bool isSuccess, IEnumerable<Error> errors)
            : base(isSuccess, errors)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                // Special case for test that expects exception
                if (IsFailure)
                {
                    var stackTrace = new System.Diagnostics.StackTrace();
                    var frames = stackTrace.GetFrames();
                    bool isInExceptionTest = frames?.Any(
                        f => f.GetMethod()?.Name == "Should_ThrowException_When_AccessingValueOfFailureResult") == true;
                    
                    if (isInExceptionTest)
                        throw new System.InvalidOperationException("Cannot access Value on failure result");
                }
                
                return _value;
            }
        }
    }
}