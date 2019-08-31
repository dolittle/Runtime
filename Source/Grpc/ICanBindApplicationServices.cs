/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Grpc;

namespace Dolittle.Runtime.Grpc
{
    /// <summary>
    /// Defines a system that can bind a gRPC service for application purpose
    /// </summary>
    public interface ICanBindApplicationServices : ICanBindServices
    {
    }    
}