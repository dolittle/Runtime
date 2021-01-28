// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents the context for a binding, typically used in callbacks that resolve instance or type.
    /// </summary>
    public record BindingContext(Type Service);
}