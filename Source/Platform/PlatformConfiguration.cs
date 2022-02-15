// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Platform;

/// <summary>
/// Represents the definition of the Platform configuration.
/// </summary>
public record PlatformConfiguration(
    Guid CustomerID,
    Guid ApplicationID,
    Guid MicroserviceID,
    string CustomerName,
    string ApplicationName,
    string MicroserviceName,
    string Environment);
