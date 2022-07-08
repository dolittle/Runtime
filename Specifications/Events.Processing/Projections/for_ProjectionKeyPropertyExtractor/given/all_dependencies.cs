// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor.given;

public class all_dependencies
{
    protected static ProjectionKeyPropertyExtractor extractor;
    Establish context = () =>
    {
        extractor = new ProjectionKeyPropertyExtractor();
    };
}