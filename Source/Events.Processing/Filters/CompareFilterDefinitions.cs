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
        public bool DefinitionsAreEqual(IFilterDefinition persisted, IFilterDefinition registered, out FilterValidationResult validationResult)
        {
            if (persisted.Partitioned != registered.Partitioned)
            {
                validationResult = new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persisted.Partitioned ? "The previous filter is partitioned while the new filter is not" : "The previous filter is not partitioned while the new filter is")}");
                return false;
            }

            if (persisted.Public != registered.Public)
            {
                validationResult = new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persisted.Public ? "The previous filter is public while the new filter is not" : "The previous filter is not public while the new filter is")}");
                return false;
            }

            validationResult = default;
            return true;
        }
    }
}
