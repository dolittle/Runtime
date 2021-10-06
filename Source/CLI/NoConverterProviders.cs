// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Types;
namespace CLI
{
    /// <summary>
    /// Represents an implementation of <see cref="InstancesOf{T}"/> of <see cref="ICanProvideConverters"/> that has no instances.
    /// </summary>
    public class NoConverterProviders : IInstancesOf<ICanProvideConverters>
    {
        /// <inheritdoc />
        public IEnumerator<ICanProvideConverters> GetEnumerator()
            => Enumerable.Empty<ICanProvideConverters>().GetEnumerator();
        
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}