// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICompareFilterDefinitions"/>.
    /// </summary>
    public class CompareFilterDefinitions : ICompareFilterDefinitions
    {
        /// <inheritdoc/>
        public FilterValidationResult DefinitionsAreEqual(IFilterDefinition persisted, IFilterDefinition registered)
        {
            if (persisted.Partitioned != registered.Partitioned)
            {
                return new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persisted.Partitioned ? "The previous filter is partitioned while the new filter is not" : "The previous filter is not partitioned while the new filter is")}");
            }

            if (persisted.Public != registered.Public)
            {
                return new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persisted.Public ? "The previous filter is public while the new filter is not" : "The previous filter is not public while the new filter is")}");
            }

            return new FilterValidationResult();
        }
    }
}
