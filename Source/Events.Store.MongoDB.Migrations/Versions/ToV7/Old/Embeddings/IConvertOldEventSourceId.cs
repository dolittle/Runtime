// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings
{
    public interface IConvertOldEventSourceId
    {
        EventSourceId Convert(Guid oldEventSource, IEnumerable<ProjectionKey> keys);
    }
}