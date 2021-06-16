// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Immutability
{
    /// <summary>
    /// Exception that gets thrown when an object is read only and one is writing to it.
    /// </summary>
    public class CannotWriteToAnImmutable : ArgumentException
    {
    }
}
