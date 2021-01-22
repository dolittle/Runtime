// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.IO.Tenants
{
    /// <summary>
    /// Exception that gets thrown when a file/path is trying to access outside the tenant sandbox.
    /// </summary>
    public class AccessOutsideSandboxDenied : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessOutsideSandboxDenied"/> class.
        /// </summary>
        /// <param name="path">The path that was attempted.</param>
        public AccessOutsideSandboxDenied(string path)
            : base($"Access outside tenant sandbox denied when trying to access '{path}'")
        {
        }
    }
}