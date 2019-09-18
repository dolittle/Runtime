/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents the delegate for when a <see cref="Client"/> gets disconnected
    /// </summary>
    /// <param name="client"><see cref="Client"/> that gets disconnected</param>
    public delegate void ClientDisconnected(Client client);
}