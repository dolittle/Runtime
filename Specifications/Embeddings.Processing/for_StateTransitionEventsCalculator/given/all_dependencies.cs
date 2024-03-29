// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Embeddings.Store;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.given;

public class all_dependencies
{
    protected static EmbeddingId identifier;
    protected static ExecutionContext execution_context;
    protected static Mock<IEmbedding> embedding;
    protected static Mock<IProjectManyEvents> project_many_events;
    protected static Mock<ICompareStates> state_comparer;
    protected static Mock<IDetectEmbeddingLoops> loop_detector;
    protected static StateTransitionEventsCalculator calculator;
    protected static CancellationToken cancellation;

    Establish context = () =>
    {
        identifier = "b368df07-e5a3-4669-95f1-19e209a7af30";
        execution_context = execution_contexts.create();
        embedding = new Mock<IEmbedding>(MockBehavior.Strict);
        project_many_events = new Mock<IProjectManyEvents>(MockBehavior.Strict);
        state_comparer = new Mock<ICompareStates>(MockBehavior.Strict);
        loop_detector = new Mock<IDetectEmbeddingLoops>(MockBehavior.Strict);
        cancellation = CancellationToken.None;
        calculator = new StateTransitionEventsCalculator(
            identifier,
            embedding.Object,
            project_many_events.Object,
            state_comparer.Object,
            loop_detector.Object,
            Mock.Of<ILogger>());
    };
}