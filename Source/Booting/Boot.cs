// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents the necessary information to perform a boot.
    /// </summary>
    public class Boot
    {
        readonly Dictionary<Type, IRepresentSettingsForBootStage> _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Boot"/> class.
        /// </summary>
        /// <param name="settings"><see cref="IEnumerable{T}"/> of <see cref="IRepresentSettingsForBootStage"/>.</param>
        public Boot(IEnumerable<IRepresentSettingsForBootStage> settings)
        {
            _settings = settings.ToDictionary(_ => _.GetType(), _ => _);
        }

        /// <summary>
        /// Get settings with a specific type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get settings for.</param>
        /// <returns>Instance of <see cref="IRepresentSettingsForBootStage"/>.</returns>
        public IRepresentSettingsForBootStage GetSettingsByType(Type type)
        {
            if (!_settings.ContainsKey(type))
            {
                var settings = Activator.CreateInstance(type) as IRepresentSettingsForBootStage;
                _settings[type] = settings;
                return settings;
            }

            return _settings[type];
        }
    }
}
