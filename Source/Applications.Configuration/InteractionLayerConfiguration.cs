// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Applications.Configuration;

/// <summary>
/// Represents the configuration for an interaction layer of the <see cref="Microservice"/>.
/// </summary>
public record InteractionLayerConfiguration(string Type, string Language, string EntryPoint, string Framework);