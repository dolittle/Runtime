using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Defines a system that can update the persisted aggregate versions from a <see cref="Commit"/>. 
/// </summary>
public interface IUpdateAggregateVersionsAfterCommit
{
    /// <summary>
    /// Updates all the aggregate root versions based on the <see cref="Commit.AggregateEvents"/>.
    /// </summary>
    /// <param name="session">The <see cref="IClientSessionHandle"/> for the MongoDB transaction.</param>
    /// <param name="commit">The <see cref="Commit"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the result of the operation.</returns>
    Task UpdateAggregateVersions(IClientSessionHandle session, Commit commit, CancellationToken cancellationToken);
}
