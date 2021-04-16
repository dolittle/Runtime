// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_starting
{
    public class twice : given.all_dependencies
    {
        static Task<Try> result;

        Because of = () =>
        {
            embedding_processor.Start(CancellationToken.None);
            result = embedding_processor.Start(CancellationToken.None);
        };

        It should_fail_the_second_time = () => result.Result.Success.ShouldBeFalse();
        It should_fail_because_it_is_already_started = () => result.Result.Exception.ShouldBeOfExactType<EmbeddingProcessorAlreadyStarted>();
    }
}