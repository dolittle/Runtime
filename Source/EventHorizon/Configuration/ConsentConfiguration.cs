// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the configuration of an event horizon consent.
/// </summary>
public class ConsentConfiguration
{
    /// <summary>
    /// Gets or sets the stream id that the consent is given for.
    /// </summary>
    public Guid Stream { get; set; }
    
    /// <summary>
    /// Gets or sets the partition of the stream that the consent is given for.
    /// </summary>
    public string Partition { get; set; }
    
    /// <summary>
    /// Gets or sets the consent id.
    /// </summary>
    public Guid Consent { get; set; }
}
