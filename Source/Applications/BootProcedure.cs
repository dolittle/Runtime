/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Represents a <see cref="ICanPerformBootProcedure">boot procedure</see> for application runtime part
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IClients _clients;

        /// <summary>
        /// Initializes a new instance of <see cref="BootProcedure"/>
        /// </summary>
        /// <param name="clients"><see cref="IClients"/> </param>
        public BootProcedure(IClients clients)
        {
            _clients = clients;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;


        /// <inheritdoc/>
        public void Perform()
        {
            ClientConnectionStateExtensions.Clients = _clients;
        }
    }
}