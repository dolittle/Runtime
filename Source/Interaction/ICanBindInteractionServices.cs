/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Hosting;

namespace Dolittle.Runtime.Interaction
{
    /// <summary>
    /// Defines a system that can bind a gRPC service for interaction purpose
    /// </summary>
    public interface ICanBindInteractionServices : ICanBindServices
    {
    }
}