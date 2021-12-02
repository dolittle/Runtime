// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;

namespace Dolittle.Runtime.Heads;

/// <summary>
/// Represents a <see cref="ICanPerformBootProcedure">boot procedure</see> for application runtime part.
/// </summary>
public class BootProcedure : ICanPerformBootProcedure
{
    readonly IConnectedHeads _connectedHeads;

    /// <summary>
    /// Initializes a new instance of the <see cref="BootProcedure"/> class.
    /// </summary>
    /// <param name="connectedHeads"><see cref="IConnectedHeads"/>.</param>
    public BootProcedure(IConnectedHeads connectedHeads)
    {
        _connectedHeads = connectedHeads;
    }

    /// <inheritdoc/>
    public bool CanPerform() => true;

    /// <inheritdoc/>
    public void Perform()
    {
        HeadConnectionStateExtensions.ConnectedHeads = _connectedHeads;
    }
}