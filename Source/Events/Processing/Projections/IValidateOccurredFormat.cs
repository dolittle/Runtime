// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Defines a validator for <see cref="OccurredFormat"/>.
/// </summary>
public interface IValidateOccurredFormat
{
    /// <summary>
    /// Checks whether the given <see cref="OccurredFormat"/> is valid, meaning it can be used to format <see cref="DateTimeOffset"/> to a string.
    /// </summary>
    /// <param name="format">The <see cref="OccurredFormat"/>to check</param>
    /// <param name="error">The outputted <see cref="Exception"/> that occurs when the <see cref="OccurredFormat"/> is not valid.</param>
    /// <returns>True if the format is valid, false if not.</returns>
    bool IsValid(OccurredFormat format, [NotNullWhen(false)] out Exception? error);
}
