using Grpc.Core;
using Grpc.Core.Testing;
using Moq;

namespace BillingAPI.IntegrationTests.Utils;

internal static class CallHelpers
{
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(TResponse response)
    {
        return TestCalls.AsyncUnaryCall(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }

    public static AsyncServerStreamingCall<TResponse> CreateAsyncStreamingCall<TResponse>(
        IEnumerable<TResponse> responses)
    {
        var mockResponseStream = new Mock<IAsyncStreamReader<TResponse>>();

        var sequentialCurrent = mockResponseStream.SetupSequence(s => s.Current);
        var sequentialMoveNext = mockResponseStream.SetupSequence(s => s.MoveNext(It.IsAny<CancellationToken>()));

        foreach (var response in responses.ToArray())
        {
            sequentialCurrent.Returns(response);
            sequentialMoveNext.Returns(Task.FromResult(true));
        }

        sequentialMoveNext.Returns(Task.FromResult(false));

        var fakeCall = TestCalls.AsyncServerStreamingCall(
            mockResponseStream.Object,
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        return fakeCall;
    }
}