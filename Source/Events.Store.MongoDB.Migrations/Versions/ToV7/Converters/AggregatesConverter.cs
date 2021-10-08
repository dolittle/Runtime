// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class AggregatesConverter : IConvertFromOldToNew<Old.Aggregates.AggregateRoot, AggregateRoot>
    {
        public AggregateRoot Convert(Old.Aggregates.AggregateRoot old)
            => new(old.EventSource.ToString(), old.AggregateType, old.Version);
    }
}