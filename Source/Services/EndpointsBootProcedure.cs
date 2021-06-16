// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IEndpoints"/>.
    /// </summary>
    public class EndpointsBootProcedure : ICanPerformBootProcedure
    {
        readonly IEndpoints _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointsBootProcedure"/> class.
        /// </summary>
        /// <param name="endpoints">Instance of <see cref="IEndpoints"/> to boot.</param>
        public EndpointsBootProcedure(IEndpoints endpoints)
        {
            _endpoints = endpoints;
        }

        /// <summary>
        /// Gets a value indicating whether or not all the <see cref="IEndpoint">endpoints</see> are ready.
        /// </summary>
        public static bool EndpointsReady { get; private set; } = false;

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _endpoints.Start();
            EndpointsReady = true;
        }
    }
}