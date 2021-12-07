// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Defines the system that knows how to perform <see cref="ICanPerformBootProcedure">boot procedures</see>.
/// </summary>
public interface IBootProcedures
{
    /// <summary>
    /// Perform all <see cref="ICanPerformBootProcedure">boot procedures</see>.
    /// </summary>
    void Perform();
}