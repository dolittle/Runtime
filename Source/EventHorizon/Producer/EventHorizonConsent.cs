// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the consent between tenants in different microservices.
    /// </summary>
    public class EventHorizonConsent : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="EventHorizonConsent"/>.
        /// </summary>
        /// <param name="reason"><see cref="Guid"/> representation.</param>
        public static implicit operator EventHorizonConsent(Guid reason) => new EventHorizonConsent { Value = reason };
    }
}