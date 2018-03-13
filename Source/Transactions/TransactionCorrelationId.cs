/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Transactions
{
    /// <summary>
    /// Represents a uniquely identifiable correlation id associated with a transaction
    /// </summary>
    public class TransactionCorrelationId : ConceptAs<Guid>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TransactionCorrelationId"/> with a unique id
        /// </summary>
        /// <returns>A new <see cref="TransactionCorrelationId"/></returns>
        public static TransactionCorrelationId New()
        {
            return new TransactionCorrelationId { Value = Guid.NewGuid() };
        }

        /// <summary>
        /// Gets the value representing a not set <see cref="TransactionCorrelationId"/>
        /// </summary>
        public static TransactionCorrelationId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="TransactionCorrelationId"/>
        /// </summary>
        /// <param name="value"><see cref="Guid"/> for the value</param>
        public static implicit operator TransactionCorrelationId(Guid value)
        {
            return new TransactionCorrelationId { Value = value };
        }
    }
}
