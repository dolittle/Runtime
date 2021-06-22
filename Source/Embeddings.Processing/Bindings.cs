// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents the bindings for embeddings processing.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<ICalculateStateTransitionEvents>().To<StateTransitionEventsCalculator>();
            builder.Bind<IUpdateEmbeddingStates>().To<EmbeddingStateUpdater>();
            builder.Bind<IDetectEmbeddingLoops>().To<EmbeddingLoopsDetector>();
            builder.Bind<ICompareStates>().To<CompareProjectionStates>();
        }
    }
}