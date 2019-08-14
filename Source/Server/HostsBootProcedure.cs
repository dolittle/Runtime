/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IHosts"/>
    /// </summary>
    public class HostsBootProcedure : ICanPerformBootProcedure
    {
        readonly IHosts _hosts;

        /// <summary>
        /// Initializes a new instance of <see cref="HostsBootProcedure"/>
        /// </summary>
        /// <param name="hosts">Instance of <see cref="IHosts"/> to boot</param>
        public HostsBootProcedure(IHosts hosts)
        {
            _hosts = hosts;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;


        /// <inheritdoc/>
        public void Perform()
        {
            _hosts.Start();
        }
    }
}