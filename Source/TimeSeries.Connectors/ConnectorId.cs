// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Defines the concept of a unique identifier for any type of connectors.
    /// </summary>
    public class ConnectorId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="ConnectorId"/>.
        /// </summary>
        /// <param name="value"><see cref="ConnectorId"/> as <see cref="Guid"/>.</param>
        public static implicit operator ConnectorId(Guid value) => new ConnectorId { Value = value };
    }
}