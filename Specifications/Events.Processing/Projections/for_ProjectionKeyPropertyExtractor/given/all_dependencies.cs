// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor.given
{
    public class all_dependencies : a_serializer
    {
        protected static ProjectionKeyPropertyExtractor extractor;
        Establish context = () =>
        {
            extractor = new ProjectionKeyPropertyExtractor();
        };
    }
}
