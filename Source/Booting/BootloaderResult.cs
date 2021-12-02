// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Represents the result of the <see cref="Bootloader"/> start.
/// </summary>
public record BootloaderResult(
    IContainer Container,
    ITypeFinder TypeFinder,
    IAssemblies Assemblies,
    IBindingCollection Bindings,
    IEnumerable<BootStageResult> BootStageResults);