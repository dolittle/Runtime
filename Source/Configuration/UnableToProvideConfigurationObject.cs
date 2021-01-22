// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="ICanProvideConfigurationObjects">provider</see> is not able to provide
    /// an instance of <see cref="IConfigurationObject"/>.
    /// </summary>
    /// <typeparam name="TProvider">Type of <see cref="ICanProvideConfigurationObjects"/>.</typeparam>
    public class UnableToProvideConfigurationObject<TProvider> : Exception
        where TProvider : ICanProvideConfigurationObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToProvideConfigurationObject{TProvider}"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of <see cref="IConfigurationObject"/> that is attempted to be provided.</param>
        public UnableToProvideConfigurationObject(Type type)
            : base($"'{typeof(TProvider).AssemblyQualifiedName}' is unable to provide '{type.GetFriendlyConfigurationName()}' - '{type.AssemblyQualifiedName}'")
        {
        }
    }
}