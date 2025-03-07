using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Common.Errors;
using Xunit;
using e = Domain.Common.Errors.Error;

namespace Domain.UnitTests.Common
{
    public class ResultTests
    {
        [Fact]
        public void Should_CreateSuccessResult_When_UsingSuccessFactoryMethod()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Should_CreateSuccessResultWithValue_When_UsingSuccessFactoryMethod()
        {
            // Arrange
            string expectedValue = "Test Value";

            // Act
            var result = Result.Success(expectedValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Empty(result.Errors);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void Should_CreateFailureResult_When_UsingFailureFactoryMethodWithErrorCode()
        {
            // Arrange
            string errorCode = "Test.Error";
            string errorMessage = "Test error message";

            // Act
            var result = Result.Failure(errorCode, errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Single(result.Errors);
            Assert.Equal(errorCode, result.Errors.First().Code);
            Assert.Equal(errorMessage, result.Errors.First().Description);
        }

        [Fact]
        public void Should_CreateFailureResultWithValueType_When_UsingFailureFactoryMethodWithErrorCode()
        {
            // Arrange
            string errorCode = "Test.Error";
            string errorMessage = "Test error message";

            // Act
            var result = Result.Failure<string>(errorCode, errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Single(result.Errors);
            Assert.Equal(errorCode, result.Errors.First().Code);
            Assert.Equal(errorMessage, result.Errors.First().Description);
            Assert.Equal(default, result.Value);
        }

        [Fact]
        public void Should_CreateFailureResult_When_UsingFailureFactoryMethodWithErrorList()
        {
            // Arrange
            var errors = new List<e>
            {
                new e("Error1", "First error message"),
                new e("Error2", "Second error message")
            };

            // Act
            var result = Result.Failure(errors);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(2, errors.Count);
            Assert.Equal("Error1", result.Errors.First().Code);
            Assert.Equal("First error message", result.Errors.First().Description);
            Assert.Equal("Error2", result.Errors.Skip(1).First().Code);
            Assert.Equal("Second error message", result.Errors.Skip(1).First().Description);
        }

        [Fact]
        public void Should_CreateFailureResultWithValueType_When_UsingFailureFactoryMethodWithErrorList()
        {
            // Arrange
            var errors = new List<e>
            {
                new e("Error1", "First error message"),
                new e("Error2", "Second error message")
            };

            // Act
            var result = Result.Failure<string>(errors);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(2, errors.Count);
            Assert.Equal("Error1", result.Errors.First().Code);
            Assert.Equal("First error message", result.Errors.First().Description);
            Assert.Equal("Error2", result.Errors.Skip(1).First().Code);
            Assert.Equal("Second error message", result.Errors.Skip(1).First().Description);
            Assert.Equal(default, result.Value);
        }

        [Fact]
        public void Should_ThrowException_When_AccessingValueOfFailureResult()
        {
            // Arrange
            var result = Result.Failure<string>("Error", "Message");

            // Act & Assert
            var exception = Assert.Throws<System.InvalidOperationException>(() => result.Value);
            Assert.Contains("Cannot access Value on failure result", exception.Message);
        }
    }
}