// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Defines a builder for a <see cref="BootStage"/>.
    /// </summary>
    public interface IBootStageBuilder
    {
        /// <summary>
        /// Gets the <see cref="IBindingProviderBuilder"/> for building specific.
        /// </summary>
        IBindingProviderBuilder Bindings { get; }

        /// <summary>
        /// Gets the current container - if not set, the exception <see cref="ContainerNotSetYet"/> will be thrown.
        /// </summary>
        /// <exception cref="ContainerNotSetYet">Thrown when the <see cref="IContainer"/> is not set yet.</exception>
        IContainer Container { get; }

        /// <summary>
        /// Called to switch to a specific <see cref="IContainer"/> - any stage beyond this stage will
        /// use the <see cref="IContainer"/> specified.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> to use.</param>
        void UseContainer(IContainer container);

        /// <summary>
        /// Associate a key with an object.
        /// </summary>
        /// <param name="key">Key for the association.</param>
        /// <param name="value">Value of the association.</param>
        /// <remarks>
        /// This is used throughout the boot process for passing information along from stages.
        /// </remarks>
        void Associate(string key, object value);

        /// <summary>
        /// Get association by key.
        /// </summary>
        /// <param name="key">Key for the association.</param>
        /// <returns>Instance associated.</returns>
        object GetAssociation(string key);

        /// <summary>
        /// Build the <see cref="BootStage"/> and return the <see cref="BootStageResult">result</see>.
        /// </summary>
        /// <returns>Resulting <see cref="BootStageResult"/>.</returns>
        BootStageResult Build();
    }
}