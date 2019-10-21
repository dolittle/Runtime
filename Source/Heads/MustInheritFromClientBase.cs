/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Exception that gets thrown if a <see cref="Type"/> does not implement <see cref="ClientBase{T}"/>
    /// </summary>
    public class MustInheritFromClientBase : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MustInheritFromClientBase"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MustInheritFromClientBase(Type type) : base($"Type '{type.AssemblyQualifiedName}' does inherit {typeof(ClientBase<>).AssemblyQualifiedName}") {}
    }
}