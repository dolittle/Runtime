using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Events.Processing.MongoDB.Fixtures;
using FluentAssertions;

namespace Events.Processing.MongoDB;

public class with_mixed_state : given.a_processor_state_repository
{
    static readonly ScopeId scope_id = new(Guid.Empty);
    readonly StreamProcessorId failing_partitioned_id = new(scope_id, Guid.NewGuid(), StreamId.EventLog);
    readonly StreamProcessorId failing_non_partitioned_id = new(scope_id, Guid.NewGuid(), StreamId.EventLog);
    readonly StreamProcessorId clean_partitioned_id = new(scope_id, Guid.NewGuid(), StreamId.EventLog);
    readonly StreamProcessorId clean_non_partitioned_id = new(scope_id, Guid.NewGuid(), StreamId.EventLog);

    public with_mixed_state(mongo_server_fixture fixture) : base(fixture)
    {
    }


    [Fact]
    public async Task mixed_states_are_returned_correctly()
    {
        var states = new Dictionary<StreamProcessorId, IStreamProcessorState>()
        {
            { failing_partitioned_id, test_data.failing_partitioned_state() },
            { failing_non_partitioned_id, test_data.failing_non_partitioned_state() },
            { clean_partitioned_id, test_data.clean_partitioned_state() },
            { clean_non_partitioned_id, test_data.clean_non_partitioned_state() },
        };


        await repository.PersistForScope(scope_id, states, CancellationToken.None);

        var results = repository.GetForScope(scope_id, CancellationToken.None);

        var resultingStates = await results.ToDictionaryAsync(
            kv => kv.Id,
            kv => kv.State);

        resultingStates.Should().BeEquivalentTo(states);
    }

    async Task<IStreamProcessorState> get(StreamProcessorId processor_id)
    {
        var results = repository.GetForScope(scope_id, CancellationToken.None);

        var result = await results.SingleAsync(it => it.Id.Equals(processor_id));
        return result.State;
    }
}