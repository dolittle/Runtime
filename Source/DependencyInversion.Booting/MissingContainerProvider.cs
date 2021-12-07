// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Booting;

/// <summary>
/// Exception that gets thrown when there are no implementations of <see cref="ICanProvideContainer"/> loaded.
/// </summary>
public class MissingContainerProvider : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingContainerProvider"/> class.
    /// </summary>
    public MissingContainerProvider()
        : base("There is no provider for an IOC Container - add a reference to an extension that provides this; http://github.com/Dolittle-extensions")
    {
    }
}