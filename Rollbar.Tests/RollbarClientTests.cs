using System;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Api;
using LightestNight.System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LightestNight.System.Logging.Rollbar.Tests
{
    public class RollbarClientTests
    {
        private readonly Mock<IApiClient> _apiClientMock = new Mock<IApiClient>();
        private readonly string _accessToken = Guid.NewGuid().ToString();
        private readonly IRollbarClient _sut;

        public RollbarClientTests()
        {
            Environment.SetEnvironmentVariable("Rollbar:AccessToken", _accessToken);
            var configurationManager = new ConfigurationManager(new ConfigurationBuilder());

            var apiClientFactoryMock = new Mock<IApiClientFactory>();
            apiClientFactoryMock.Setup(apiClientFactory => apiClientFactory.Create(It.IsAny<string>()))
                .Returns(_apiClientMock.Object);
            
            _sut = new RollbarClient(apiClientFactoryMock.Object, configurationManager);
        }

        [Fact]
        public async Task Should_Log_LogData_To_Rollbar()
        {
            // Arrange
            var logData = new LogData
            {
                Title = "Test Title",
                Message = "Test Message",
                Function = "Test Function",
                Severity = LogLevel.Information
            };
            
            // Act
            await _sut.Log(logData);
            
            // Assert
            _apiClientMock.Verify(apiClient => apiClient.Post(It.Is<ApiRequest>(req =>
                    req.Body.GetType() == typeof(RollbarPayload) &&
                    ((RollbarPayload) req.Body).AccessToken == _accessToken &&
                    ((RollbarPayload) req.Body).Data.Body.Title == logData.Title &&
                    ((RollbarPayload) req.Body).Data.Body.Uuid != default &&
                    ((RollbarPayload) req.Body).Data.Body.Message.Body == logData.Message &&
                    ((RollbarPayload) req.Body).Data.Body.Message.Metadata[nameof(LogData.Title)] == logData.Title &&
                    ((RollbarPayload) req.Body).Data.Body.Message.Metadata[nameof(LogData.Message)] == logData.Message &&
                    ((RollbarPayload) req.Body).Data.Body.Message.Metadata[nameof(LogData.Function)] == logData.Function &&
                    ((RollbarPayload) req.Body).Data.Body.Message.Metadata[nameof(LogData.Severity)] == logData.Severity.ToString()), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}