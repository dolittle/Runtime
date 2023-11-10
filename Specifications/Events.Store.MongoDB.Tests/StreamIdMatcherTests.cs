using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using FluentAssertions;
using Xunit;

namespace Events.Store.MongoDB.Tests;

public class StreamIdMatcherTests
{
    [Theory]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-stream-6a080414-d493-4ce1-a11b-bd60208b9d7a")]
    [InlineData("x-16fd3dcf-a457-4e76-917e-5049ef49bfd3-stream-6a080414-d493-4ce1-a11b-bd60208b9d7b")]
    
    public void VerifyMatchesScopedStream(string input)
    {
        StreamIdMatcher.IsScopedStream(input).Should().BeTrue();
    }
    
    [Theory]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-event-log")]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd4-event-log")]
    
    public void VerifyMatchesScopedEventLog(string input)
    {
        StreamIdMatcher.IsScopedEventLog(input).Should().BeTrue();
    }
    
    
    [Theory]
    [InlineData("event-log")]
    [InlineData("stream-6a080414-d493-4ce1-a11b-bd60208b9d7a")]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-event-log")]
    [InlineData("x-16fd3dcf-a457-4e76-917e-5049ef49bfd3-stream-6a080414-d493-4ce1-a11b-bd60208b9d7b")]
    public void VerifyMatches(string input)
    {
        StreamIdMatcher.IsStreamOrEventLog(input).Should().BeTrue();
    }

    [Theory]
    [InlineData("stream-processor-states")]
    [InlineData("stream-definitions")]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-stream-definitions")]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-stream-processor-states")]
    [InlineData("x-06fd3dcf-a457-4e76-917e-5049ef49bfd3-subscription-states")]
    public void DoesNotMatchOtherCollections(string input)
    {
        StreamIdMatcher.IsStreamOrEventLog(input).Should().BeFalse();
    }
}