// using System.Threading.Channels;
// using Dolittle.Runtime.Events.Processing;
// using Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
// using Dolittle.Runtime.Events.Processing.Streams;
// using Dolittle.Runtime.Events.Store;
// using Dolittle.Runtime.Events.Store.Streams;
// using Moq;
//
// namespace Events.Processing.Tests;
//
// public class PartitionedProcessorTests
// {
//     const string reason = "some reason";
//     PartitionId first_partition_id = "first partition";
//     PartitionId second_partition_id = "second partition";
//     StreamEvent first_event;
//     StreamEvent second_event;
//     StreamEvent third_event;
//     StreamEvent fourth_event;
//     Mock<IEventProcessor> event_processor;
//     private Channel<StreamEvent> event_stream;
//
//     PartitionedProcessorTests()
//     {
//         first_event = new StreamEvent(committed_events.single(), StreamPosition.Start, Guid.NewGuid(), first_partition_id, true);
//         second_event = new StreamEvent(committed_events.single(), 1u, Guid.NewGuid(), second_partition_id, true);
//         third_event = new StreamEvent(committed_events.single(), 2u, Guid.NewGuid(), second_partition_id, true);
//         fourth_event = new StreamEvent(committed_events.single(), 3u, Guid.NewGuid(), second_partition_id, true);
//         event_processor = new Mock<IEventProcessor>();
//         event_processor
//             .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), second_partition_id, Moq.It.IsAny<Dolittle.Runtime.Execution.ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
//             .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
//         event_processor
//             .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), first_partition_id, Moq.It.IsAny<Dolittle.Runtime.Execution.ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
//             .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason, true, TimeSpan.FromMilliseconds(1))));
//         event_stream = Channel.CreateUnbounded<StreamEvent>();
//         var writer = event_stream.Writer;
//         writer.WriteAsync(first_event).GetAwaiter().GetResult();
//         writer.WriteAsync(second_event).GetAwaiter().GetResult();
//         writer.WriteAsync(third_event).GetAwaiter().GetResult();
//         writer.WriteAsync(fourth_event).GetAwaiter().GetResult();
//     }
//     
//     static readonly EventProcessorId eventProcessorId = new EventProcessorId(Guid.NewGuid());
//     static readonly StreamProcessorId streamProcessorId = new StreamProcessorId(ScopeId.Default, eventProcessorId.Value, eventProcessorId.Value);        
//
//     
//     [Fact]
//     public void ShouldRetryFailedEvents()
//     {
//         // new PartitionedProcessor(streamProcessorId, event_processor.Object,  )
//         
//         
//         
//     }
// }