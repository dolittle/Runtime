// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when there are no <see cref="ICanValidateFilterFor{TDefinition}" /> for the given <see cref="IFilterDefinition" /> <see cref="Type"/>.
    /// </summary>
    public class CannotValidateFilterWithDefinitionType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotValidateFilterWithDefinitionType"/> class.
        /// </summary>
        /// <param name="filterDefinitionType">The <see cref="Type" /> of the <see cref="IFilterDefinition" /> that there are no filter validator for.</param>
        public CannotValidateFilterWithDefinitionType(Type filterDefinitionType)
            : base($"There are no filter validator for filter definition {filterDefinitionType.FullName}")
        {
        }
    }
}