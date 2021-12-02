// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Applications.Configuration;

/// <summary>
/// Represents the configuration for the <see cref="Microservice"/> core.
/// </summary>
public record CoreConfiguration(string Language, string EntryPoint);