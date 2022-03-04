// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_deleting;

public class and_request_is_sent_to_the_wrong_processor : given.all_dependencies_and_a_key
{
    static Task task;

    private Establish context = () =>
    {
        task = embedding_processor.Start(cancellation_token);
    };
    
    static Exception result;

    Because of = () => result = Catch.Exception(() => embedding_processor.Delete(key, execution_context with{ Tenant = "36b31638-e7b0-4e14-9116-92267237d8e4"}, cancellation_token).GetAwaiter().GetResult());
    
    It should_still_be_running = () => task.Status.ShouldEqual(TaskStatus.WaitingForActivation);
    It should_fail = () => result.ShouldBeOfExactType<EmbeddingRequestWorkScheduledForWrongTenant>();
}