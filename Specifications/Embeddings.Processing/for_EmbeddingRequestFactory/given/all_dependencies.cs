// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.given
{
    public class all_dependencies
    {
        protected static EmbeddingRequestFactory factory;

        Establish context = () =>
        {
            factory = new EmbeddingRequestFactory();
        };
    }
}
