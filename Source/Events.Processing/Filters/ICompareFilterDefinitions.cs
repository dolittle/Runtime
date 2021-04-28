// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that can compare instances of <see cref="IFilterDefinition"/> and evaluate if they are equal or not.
    /// </summary>
    public interface ICompareFilterDefinitions
    {
        /// <summary>
        /// Checks whether the persisted and registered instance of the filter definition are equal.
        /// </summary>
        /// <param name="persisted">The persisted <see cref="IFilterDefinition"/>.</param>
        /// <param name="registered">The registered <see cref="IFilterDefinition"/>.</param>
        /// <param name="validationResult">If the filter definitions are unequal, a <see cref="FilterValidationResult"/> that explains the difference.</param>
        /// <returns>True if the definitions are equal, false if not.</returns>
        bool DefinitionsAreEqual(IFilterDefinition persisted, IFilterDefinition registered, out FilterValidationResult validationResult);
    }
}
