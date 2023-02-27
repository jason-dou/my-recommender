using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Moq;
using My.Function;
using Xunit;

namespace My.Function;

public class RecommendationUnitTest
{
    [Fact]
    public async Task NoToken_ShouldBeUnauthorized()
    {
        var mockStream = new MockBlobStream();
        var req = new Mock<HttpRequest>();
        var log = new Mock<ILogger>();
        var executionContext = new Microsoft.Azure.WebJobs.ExecutionContext();

        var result = await Recommendation.Run(req.Object, mockStream, log.Object, executionContext) as StatusCodeResult;

        Assert.Equal(401, result.StatusCode);
    }
}