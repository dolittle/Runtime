// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Projections.Store.Services
{
    /// <summary>
    /// Exception that gets thrown when attempting to convert a <see cref="ProjectionCurrentStateType"/> that does not have a known protobuf representation.
    /// </summary>
    public class UnknownProjectionCurrentStateType : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="UnknownProjectionCurrentStateType"/> class.
        /// </summary>
        /// <param name="type">The current state type that is not known.</param>
        public UnknownProjectionCurrentStateType(ProjectionCurrentStateType type)
            : base($"ProjectionCurrentStateType {type} does not have a known protobuf representation")
        {
        }
    }
}