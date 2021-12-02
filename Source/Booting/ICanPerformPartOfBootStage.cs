// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Defines the basis of a system that is capable of performing operations as part of a <see cref="BootStage"/>.
/// </summary>
public interface ICanPerformPartOfBootStage
{
    /// <summary>
    /// Gets the <see cref="BootStage"/> it supports.
    /// </summary>
    BootStage BootStage { get; }
}

/// <summary>
/// Defines a system that is capable of performing operations as part of a <see cref="BootStage"/>.
/// </summary>
/// <typeparam name="T"><see cref="IRepresentSettingsForBootStage"/> type.</typeparam>
public interface ICanPerformPartOfBootStage<T> : ICanPerformPartOfBootStage
    where T : IRepresentSettingsForBootStage
{
    /// <summary>
    /// Method that gets called when system wants you to perform operations.
    /// </summary>
    /// <param name="settings"><see cref="IRepresentSettingsForBootStage">Settings</see> for the <see cref="BootStage"/>.</param>
    /// <param name="builder"><see cref="IBootStageBuilder"/> for the <see cref="BootStage"/> you represent.</param>
    void Perform(T settings, IBootStageBuilder builder);
}