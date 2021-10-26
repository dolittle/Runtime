// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.AggregateRoots;

namespace Dolittle.Runtime.Aggregates
{
    public interface IAggregates
    {
        Task<IEnumerable<(AggregateRoot, IEnumerable<Aggregate>)>> GetAll();

        Task<IEnumerable<Aggregate>> GetFor(AggregateRoot aggregateRoot);
    }
}
