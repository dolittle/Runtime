/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Bootstrapping;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IInteractionServer"/>
    /// </summary>
    public class ManagementServerBootProcedure : ICanPerformBootProcedure
    {
        readonly IManagementServer _managementServer;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagementServerBootProcedure"/>
        /// </summary>
        /// <param name="managementServer">Instance of <see cref="IManagementServer"/> to boot</param>
        public ManagementServerBootProcedure(IManagementServer managementServer)
        {
            _managementServer = managementServer;
        }

        /// <inheritdoc/>
        public void Perform()
        {
            _managementServer.Start();            
        }
    }
}