// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using Dolittle.Runtime.Domain.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.when_initializing;

public class twice : given.all_dependencies
{
    static Exception exception;

    Establish context = () =>
    {
        tenants.SetupGet(_ => _.All).Returns(new ObservableCollection<TenantId>(new[] { new TenantId(Guid.NewGuid()) }));
        stream_processor.Initialize().GetAwaiter().GetResult();
    };

    Because of = () => exception = Catch.Exception(() => stream_processor.Initialize().GetAwaiter().GetResult());

    It should_fail_because_it_is_already_initialized = () => exception.ShouldBeOfExactType<StreamProcessorAlreadyInitialized>();
}