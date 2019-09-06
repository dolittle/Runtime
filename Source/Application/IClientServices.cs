/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClientServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T GetService<T>() where T:ClientBase<T>;
    }
}