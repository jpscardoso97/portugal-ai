namespace App.Test.Services;
    
using App.Services;
using App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

public class ChatServiceTests
{
    private readonly ChatService _chatService;
    private readonly Mock<ILogger<ChatService>> _loggerMock;
    private readonly Mock<ISemanticKernelService> _semanticKernelServiceMock;

    public ChatServiceTests()
    {
        _loggerMock = new();
        _semanticKernelServiceMock = new();
        _chatService = new(_loggerMock.Object, _semanticKernelServiceMock.Object);
    }

    [Fact] public async Task GetInitialResponse_ValidLocation_ReturnsCleanResponse()
    {
        // Arrange
        string initialInput = "Tell me about Aveiro";
        string extractedLocation = "aveiro";
        string recommendations = "Aveiro is a beautiful city in Portugal.";
        string rawCompletion = "Sure, Aveiro is a beautiful city in Portugal.<|eot_id|>";
        string cleanCompletion = "Sure, Aveiro is a beautiful city in Portugal.";

        _semanticKernelServiceMock.Setup(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()))
            .ReturnsAsync(extractedLocation);

        _semanticKernelServiceMock.Setup(s => s.GetRecommendationsForLocationAsync(It.IsAny<string>()))
            .ReturnsAsync(recommendations);

        _semanticKernelServiceMock.Setup(s => s.GetCompletionAsync(It.IsAny<string>()))
            .ReturnsAsync(rawCompletion);

        // Act
        var result = await _chatService.GetInitialResponse(initialInput);

        // Assert
        Assert.Equal(cleanCompletion, result);
        _semanticKernelServiceMock.Verify(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()), Times.Once);
        _semanticKernelServiceMock.Verify(s => s.GetRecommendationsForLocationAsync(It.IsAny<string>()), Times.Once);
        _semanticKernelServiceMock.Verify(s => s.GetCompletionAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetInitialResponse_InvalidLocation_LogsErrorAndReturnsResponse()
    {
        // Arrange
        string initialInput = "Tell me about a non-existent place";
        string extractedLocation = "";
        string recommendations = "Sorry, no information available.";
        string completion = "Sorry, no information available.";
        
        _semanticKernelServiceMock.Setup(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()))
            .ReturnsAsync(extractedLocation);

        _semanticKernelServiceMock.Setup(s => s.GetRecommendationsForLocationAsync(It.IsAny<string>()))
            .ReturnsAsync(recommendations);

        _semanticKernelServiceMock.Setup(s => s.GetCompletionAsync(It.IsAny<string>()))
            .ReturnsAsync(completion);

        // Act
        var result = await _chatService.GetInitialResponse(initialInput);

        // Assert
        Assert.Equal(completion, result);
        _semanticKernelServiceMock.Verify(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()), Times.Once);
        _semanticKernelServiceMock.Verify(s => s.GetRecommendationsForLocationAsync(It.IsAny<string>()), Times.Once);
        _semanticKernelServiceMock.Verify(s => s.GetCompletionAsync(It.IsAny<string>()), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
