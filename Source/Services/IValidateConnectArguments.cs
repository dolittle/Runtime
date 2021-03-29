// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a validator that can validate <typeparamref name="TConnectArguments"/>.
    /// </summary>
    /// <typeparam name="TConnectArguments">Type of the runtime representation of the arguments that are sent along with the initial Connect call.</typeparam>
    public interface IValidateConnectArguments<TConnectArguments>
        where TConnectArguments : class
    {
        /// <summary>
        /// Validates the <typeparamref name="TConnectArguments"/>.
        /// /// </summary>
        /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to validate.</param>
        /// <returns>The converted <typeparamref name="TRuntimeConnectArguments"/>.</returns>
        ConnectArgumentsValidationResult ValidateConnectArguments(TConnectArguments arguments);
    }
}
