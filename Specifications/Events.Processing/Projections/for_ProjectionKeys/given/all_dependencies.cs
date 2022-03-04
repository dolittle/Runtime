// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.given;

public class all_dependencies
{
    protected static ProjectionKeys projection_keys;
    Establish context = () =>
    {
        projection_keys = new ProjectionKeys(new ProjectionKeyPropertyExtractor(), Mock.Of<ILogger>());
    };
}