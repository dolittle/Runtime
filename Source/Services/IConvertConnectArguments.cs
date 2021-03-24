// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a converter that converts the <typeparamref name="TConnectArguments"/> to <typeparamref name="TRuntimeConnectArguments"/>.
    /// </summary>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TRuntimeConnectArguments">Type of the runtime representation of the connection arguments.</typeparam>
    public interface IConvertConnectArguments<in TConnectArguments, out TRuntimeConnectArguments>
        where TConnectArguments : class
        where TRuntimeConnectArguments : class
    {
        /// <summary>
        /// Converts the <typeparamref name="TConnectArguments"/> to <typeparamref name="TRuntimeConnectArguments"/>.
        /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to convert.</param>
        /// <returns>The converted <typeparamref name="TRuntimeConnectArguments"/>.</returns>
        TRuntimeConnectArguments ConvertConnectArguments(TConnectArguments arguments);
    }
}
