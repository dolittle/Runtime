// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Booting
{
    /// <summary>
    /// Represents the result of booting.
    /// </summary>
    public record BootResult(IContainer Container, IBindingCollection Bindings);
}