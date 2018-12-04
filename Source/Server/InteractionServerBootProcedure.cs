/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IInteractionServer"/>
    /// </summary>
    public class InteractionServerBootProcedure : ICanPerformBootProcedure
    {
        readonly IInteractionServer _interactionServer;

        /// <summary>
        /// Initializes a new instance of <see cref="InteractionServerBootProcedure"/>
        /// </summary>
        /// <param name="interactionServer">Instance of <see cref="IInteractionServer"/> to boot</param>
        public InteractionServerBootProcedure(IInteractionServer interactionServer)
        {
            _interactionServer = interactionServer;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;


        /// <inheritdoc/>
        public void Perform()
        {
            _interactionServer.Start();            
        }
    }
}