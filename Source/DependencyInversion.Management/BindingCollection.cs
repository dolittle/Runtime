// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using contracts::Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.DependencyInversion.Management
{
    /// <summary>
    /// Represents all <see cref="Binding">bindings</see> in the system.
    /// </summary>
    public class BindingCollection : IEnumerable<Binding>
    {
        readonly List<Binding> _bindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingCollection"/> class.
        /// </summary>
        /// <param name="bindings">Collection of all <see cref="Binding">bindings</see>.</param>
        public BindingCollection(IEnumerable<Dolittle.DependencyInversion.Binding> bindings) => _bindings = new List<Binding>(bindings.Select(_ => _.ToProtobuf()));

        /// <inheritdoc/>
        public IEnumerator<Binding> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }
    }
}