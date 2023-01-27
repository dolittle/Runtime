// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Driver;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.MongoDB.for_AsyncCursorSourceExtensions;

public class when_iterating : given.an_async_cursor_source
{
    static Mock<Action> dispose;
    static IAsyncCursorSource<string> source;

    private Establish context = () =>
    {
        dispose = new Mock<Action>();
        source = CreateAsyncCursorSourceThatYields(dispose.Object,
        new[] { "first", "second", "third" },
        new[] { "fourth" },
        new[] { "fifth", "sixth" }
        );
    };

    static IEnumerable<string> result;

    Because of = () => result = source.ToAsyncEnumerable().ToEnumerable().ToList();

    private It should_return_the_correct_sequence = () => result.ShouldContainOnly("first", "second", "third", "fourth", "fifth", "sixth");
    private It should_dispose_of_the_cursor = () => dispose.Verify(_ => _(), Times.Once);
}