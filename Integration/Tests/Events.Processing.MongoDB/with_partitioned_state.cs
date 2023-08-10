using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Events.Processing.MongoDB.Fixtures;
using FluentAssertions;

namespace Events.Processing.MongoDB;

public class with_partitioned_state : given.a_processor_state_repository
{
    static readonly ScopeId scope_id = new(Guid.Empty);
    readonly EventProcessorId event_processor_id = Guid.NewGuid();

    StreamProcessorId stream_processor_id => new(scope_id, event_processor_id, StreamId.EventLog);

    public with_partitioned_state(mongo_server_fixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task clean_partitioned_state_is_returned_correctly()
    {
        var id = stream_processor_id;
        var partitionedState = test_data.clean_partitioned_state();

        await persist(id, partitionedState);
        var resultState = await get(id);

        resultState.Should().NotBeNull();
        resultState.Should().BeEquivalentTo(partitionedState);
    }
    
    [Fact]
    public async Task failing_partitioned_state_is_returned_correctly()
    {


        var id = stream_processor_id;

        var partitionedState = test_data.failing_partitioned_state();

        await persist(id, partitionedState);
        var resultState = await get(id);

        resultState.Should().NotBeNull();
        resultState.Should().BeEquivalentTo(partitionedState);
    }

    async Task<IStreamProcessorState> get(StreamProcessorId processor_id)
    {
        var results = repository.GetForScope(scope_id, CancellationToken.None);

        var result = await results.SingleAsync(it => it.Id.Equals(processor_id));
        return result.State;
    }


    async Task persist(StreamProcessorId id, IStreamProcessorState state)
    {
        await repository.PersistForScope(scope_id, new Dictionary<StreamProcessorId, IStreamProcessorState>
        {
            { id, state }
        }, CancellationToken.None);
    }
}