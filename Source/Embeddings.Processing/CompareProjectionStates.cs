// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <inheritdoc/>
    public class CompareProjectionStates : ICompareStates
    {
        /// <inheritdoc/>
        public Try<bool> TryCheckEquality(ProjectionState left, ProjectionState right)
            => left == right ? true : Try<bool>.Failed();
    }
}
