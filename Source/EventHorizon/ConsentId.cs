// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents the identifier of a consent between tenants in two microservices for a specific <see cref="Subscription" />..
    /// </summary>
    public class ConsentId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="ConsentId"/>.
        /// </summary>
        /// <param name="reason"><see cref="Guid"/> representation.</param>
        public static implicit operator ConsentId(Guid reason) => new ConsentId { Value = reason };
    }
}