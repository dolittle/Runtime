// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Types
{
    /// <summary>
    /// Defines something that can discover implementations of types enable enumeration of these.
    /// </summary>
    /// <typeparam name="T">Base type to discover for - must be an abstract class or an interface.</typeparam>
    public interface IImplementationsOf<T> : IEnumerable<Type>
        where T : class
    {
    }
}