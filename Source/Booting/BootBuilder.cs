// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents an implementation of <see cref="IBootBuilder"/>.
    /// </summary>
    public class BootBuilder : IBootBuilder
    {
        readonly Dictionary<Type, IRepresentSettingsForBootStage> _settings = new Dictionary<Type, IRepresentSettingsForBootStage>();

        /// <inheritdoc/>
        public Boot Build()
        {
            var boot = new Boot(_settings.Values);
            return boot;
        }

        /// <inheritdoc/>
        public void Set<TTarget>(Expression<Func<TTarget, object>> propertyExpression, object value)
            where TTarget : class, IRepresentSettingsForBootStage, new()
        {
            var type = typeof(TTarget);
            TTarget instance;
            if (_settings.ContainsKey(type))
            {
                instance = _settings[type] as TTarget;
            }
            else
            {
                instance = new TTarget();
                _settings[type] = instance;
            }

            var propertyInfo = propertyExpression.GetPropertyInfo();
            propertyInfo.GetSetMethod(true).Invoke(instance, new[] { value });
        }
    }
}
