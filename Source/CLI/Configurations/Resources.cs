// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Rudimentary;
namespace CLI.Configurations
{
    public class Resources : IResources
    {
        readonly IConfigurations _configurations;
        static readonly RuntimeConfigurationName _resourcesName = "resources.json";

        /// <summary>
        /// Initialises an instance of the <see cref="Resources"/> class.
        /// </summary>
        /// <param name="configurations"></param>
        public Resources(IConfigurations configurations)
        {
            _configurations = configurations;
        }

        /// <inheritdoc />
        public Try<ResourceConfigurationsByTenant> TryGet(RuntimeConfigurationName configurationName)
            => _configurations.TryGet<ResourceConfigurationsByTenant>(string.IsNullOrEmpty(configurationName.Value) ? _resourcesName : configurationName);
    }
}