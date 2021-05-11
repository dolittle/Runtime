// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Processing
{

    /// <summary>
    /// Represents an implementation of <see cref="ICompareStates" />.
    /// </summary>
    public class CompareProjectionStates : ICompareStates
    {
        /// <inheritdoc/>
        public Try<bool> TryCheckEquality(ProjectionState left, ProjectionState right)
        {
            try
            {
                var leftObject = JObject.Parse(left.Value);
                var rightObject = JObject.Parse(right.Value);
                return JToken.DeepEquals(leftObject, rightObject);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
