/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Grpc;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Defines a system that can bind a gRPC service for management purpose
    /// </summary>
    public interface ICanBindManagementServices : ICanBindServices
    {
    }
}