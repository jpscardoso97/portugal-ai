namespace App.Test.Services;

using App.Services;
using App.Services.Interfaces;
using Dto;
using Microsoft.Extensions.Logging;
using Moq;

public class ChatServiceTests
{
    private readonly ChatService _chatService;
    private readonly Mock<ILogger<ChatService>> _loggerMock;
    private readonly Mock<ISemanticKernelService> _semanticKernelServiceMock;
    private readonly Mock<IVectorDbService> _dbServiceMock;

    private static readonly IEnumerable<VectorDbSearchResult> Recommendations = new List<VectorDbSearchResult>
    {
        new() { Text = "Aveiro is a beautiful city in Portugal." }
    };
    
    private static readonly IEnumerable<VectorDbSearchResult> GenericRecommendations = new List<VectorDbSearchResult>
    {
        new() { Text = "Portugal is one of the hidden gems in western europe" }
    };
    
    public ChatServiceTests()
    {
        _loggerMock = new();
        _semanticKernelServiceMock = new();
        _dbServiceMock = new();
        _chatService = new(_loggerMock.Object, _semanticKernelServiceMock.Object, _dbServiceMock.Object);
        
        _dbServiceMock.Setup(s => s.SearchRecommendationsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Recommendations);
        
        _dbServiceMock.Setup(s => s.SearchRecommendationsAsync(It.IsAny<string>()))
            .ReturnsAsync(GenericRecommendations);
    }

    [Fact]
    public async Task GetInitialResponse_ValidLocation_ReturnsCleanResponse()
    {
        // Arrange
        string initialInput = "Tell me about Aveiro";
        string extractedLocation = "aveiro";
        
        string rawCompletion = "Sure, Aveiro is a beautiful city in Portugal.<|eot_id|>";
        string cleanCompletion = "Sure, Aveiro is a beautiful city in Portugal.";

        _semanticKernelServiceMock.Setup(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()))
            .ReturnsAsync(extractedLocation);

        _semanticKernelServiceMock.Setup(s => s.GetCompletionAsync(It.IsAny<string>()))
            .ReturnsAsync(rawCompletion);

        // Act
        var result = await _chatService.GetInitialResponse(initialInput);

        // Assert
        Assert.Equal(cleanCompletion, result);
        _semanticKernelServiceMock.Verify(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()), Times.Once);
        _dbServiceMock.Verify(s => s.SearchRecommendationsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _semanticKernelServiceMock.Verify(s => s.GetCompletionAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetInitialResponse_InvalidLocation_LogsErrorAndReturnsResponse()
    {
        // Arrange
        string initialInput = "Tell me about a non-existent place";
        string extractedLocation = "";
        string completion = "Sorry, no information available.";

        _semanticKernelServiceMock.Setup(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()))
            .ReturnsAsync(extractedLocation);

        _semanticKernelServiceMock.Setup(s => s.GetCompletionAsync(It.IsAny<string>()))
            .ReturnsAsync(completion);

        // Act
        var result = await _chatService.GetInitialResponse(initialInput);

        // Assert
        Assert.Equal(completion, result);
        _semanticKernelServiceMock.Verify(s => s.ExtractLocationFromInputAsync(It.IsAny<string>()), Times.Once);
        _dbServiceMock.Verify(s => s.SearchRecommendationsAsync(It.IsAny<string>()), Times.Once);
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