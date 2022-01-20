// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Booting.Stages;

/// <summary>
/// Represents the settings for <see cref="BootStage.Basics"/> stage.
/// </summary>
public class BasicsSettings : IRepresentSettingsForBootStage
{
    /// <summary>
    /// Gets the <see cref="Environment"/> we're running in.
    /// </summary>
    public Environment Environment { get; internal set; }

    /// <summary>
    /// Gets the entry <see cref="Assembly"/>.
    /// </summary>
    public Assembly EntryAssembly { get; internal set; }
}