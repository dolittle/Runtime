// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings
{
    public class CannotComputeEventSourceId : Exception
    {
        public CannotComputeEventSourceId(Guid oldEventSource)
            : base($"Cannot compute event source id from {oldEventSource}")
        {
        }
    }
}