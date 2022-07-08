// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Exception that gets thrown when attempting to call a method on <see cref="ScopedHostBuilder"/> that is not supported.
/// </summary>
public class OperationNotSupportedOnScopedHostBuilder : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationNotSupportedOnScopedHostBuilder"/> class.
    /// </summary>
    /// <param name="method">The method that was called.</param>
    public OperationNotSupportedOnScopedHostBuilder(string method)
        : base($"The method {method} is not supported on a ${nameof(ScopedHostBuilder)}")
    {
    }
}
