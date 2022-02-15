// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Resources;

/// <summary>
/// Represents the configuration for resources per tenant.
/// </summary>
public class ResourceConfigurationsByTenant : ReadOnlyDictionary<Guid, ReadOnlyDictionary<string, dynamic>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceConfigurationsByTenant"/> class.
    /// </summary>
    /// <param name="dictionary">The configuration to initialize with.</param>
    public ResourceConfigurationsByTenant(
        IDictionary<Guid, ReadOnlyDictionary<string, dynamic>> dictionary)
        : base(dictionary)
    {
    }
}
