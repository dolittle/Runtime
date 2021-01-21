// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Types
{
    /// <summary>
    /// Represents an implementation of <see cref="ITypeFinder"/>.
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        readonly IContractToImplementorsMap _contractToImplementorsMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFinder"/> class.
        /// </summary>
        /// <param name="contractToImplementorsMap"><see cref="IContractToImplementorsMap"/> for keeping track of the relationship between contracts and implementors.</param>
        public TypeFinder(IContractToImplementorsMap contractToImplementorsMap)
        {
            _contractToImplementorsMap = contractToImplementorsMap;
        }

        /// <inheritdoc/>
        public IEnumerable<Type> All => _contractToImplementorsMap.All;

        /// <inheritdoc/>
        public Type FindSingle<T>()
        {
            var type = FindSingle(typeof(T));
            return type;
        }

        /// <inheritdoc/>
        public IEnumerable<Type> FindMultiple<T>()
        {
            var typesFound = FindMultiple(typeof(T));
            return typesFound;
        }

        /// <inheritdoc/>
        public Type FindSingle(Type type)
        {
            var typesFound = _contractToImplementorsMap.GetImplementorsFor(type);
            ThrowIfMultipleTypesFound(type, typesFound);
            return typesFound.SingleOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<Type> FindMultiple(Type type)
        {
            var typesFound = _contractToImplementorsMap.GetImplementorsFor(type);
            return typesFound;
        }

        /// <inheritdoc/>
        public Type FindTypeByFullName(string fullName)
        {
            var typeFound = _contractToImplementorsMap.All.Where(t => t.FullName == fullName).SingleOrDefault();
            ThrowIfTypeNotFound(fullName, typeFound);
            return typeFound;
        }

        void ThrowIfMultipleTypesFound(Type type, IEnumerable<Type> typesFound)
        {
            if (typesFound.Count() > 1)
                throw new MultipleTypesFound(type, typesFound);
        }

        void ThrowIfTypeNotFound(string fullName, Type typeFound)
        {
            if (typeFound == null) throw new UnableToResolveTypeByName(fullName);
        }
    }
}
