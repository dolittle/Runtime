// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Exception that gets thrown when attempting to convert a <see cref="ProjectionEventKeySelectorType"/> that does not have a known protobuf representation.
    /// </summary>
    public class UnknownProjectionKeySelectorType : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="UnknownProjectionKeySelectorType"/> class.
        /// </summary>
        /// <param name="type">The key selector type that is not known.</param>
        public UnknownProjectionKeySelectorType(ProjectionEventKeySelectorType type)
            : base($"ProjectionEventKeySelectorType {type} does not have a known protobuf representation")
        {
        }
    }
}