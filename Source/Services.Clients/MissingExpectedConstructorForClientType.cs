// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Exception that gets thrown when a client type does not have a constructor that takes <see cref="CallInvoker"/> as the only argument.
    /// </summary>
    public class MissingExpectedConstructorForClientType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingExpectedConstructorForClientType"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> that does not have the expected constructor.</param>
        public MissingExpectedConstructorForClientType(Type type)
            : base($"Type '{type.AssemblyQualifiedName}' does not have a constructor that takes a CallInvoker as the only argument.")
        {
        }
    }
}