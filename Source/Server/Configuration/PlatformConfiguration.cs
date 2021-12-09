// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Server.Configuration;

/// <summary>
/// Represents the .
/// </summary>
[Name("platform")]
public record PlatformConfiguration(
    Guid ApplicationID,
    Guid MicroserviceID,
    Guid CustomerID,
    string ApplicationName,
    string MicroserviceName,
    string CustomerName,
    string Environment) : IConfigurationObject;
