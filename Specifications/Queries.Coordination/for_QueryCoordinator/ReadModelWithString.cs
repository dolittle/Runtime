// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ReadModels;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class ReadModelWithString : IReadModel
    {
        public string Content { get; set; }
    }
}
