// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.DependencyInversion.Management
{
    /// <summary>
    /// Extensions for working with bindings.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Convert a native <see cref="Binding"/> to Grpc representation.
        /// </summary>
        /// /// <param name="binding"><see cref="Binding"/> to convert.</param>
        /// <returns>Converted <see cref="Binding"/>.</returns>
        public static Binding ToProtobuf(this Dolittle.DependencyInversion.Binding binding)
        {
            return new Binding
            {
                Service = binding.Service.AssemblyQualifiedName,
                Strategy = binding.Strategy.GetType().Name,
                StrategyData = binding.Strategy.GetTargetType()?.AssemblyQualifiedName ?? "N/A",
                Scope = binding.Scope.GetType().Name
            };
        }
    }
}