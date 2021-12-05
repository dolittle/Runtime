// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.Types;

/// <summary>
/// Represents an implementation of <see cref="IContractToImplementorsMap"/>.
/// </summary>
public class ContractToImplementorsMap : IContractToImplementorsMap
{
    readonly ConcurrentDictionary<Type, ConcurrentBag<Type>> _contractsAndImplementors = new();
    readonly ConcurrentDictionary<Type, Type> _allTypes = new();

    /// <inheritdoc/>
    public IDictionary<Type, IEnumerable<Type>> ContractsAndImplementors
        => _contractsAndImplementors.ToDictionary(_ => _.Key, _ => _.Value.AsEnumerable());

    /// <inheritdoc/>
    public IEnumerable<Type> All => _allTypes.Keys;

    /// <inheritdoc/>
    public void Feed(IEnumerable<Type> types)
    {
        MapTypes(types);
        AddTypesToAllTypes(types);
    }

    /// <inheritdoc/>
    public IEnumerable<Type> GetImplementorsFor<T>() => GetImplementorsFor(typeof(T));

    /// <inheritdoc/>
    public IEnumerable<Type> GetImplementorsFor(Type contract) => GetImplementingTypesFor(contract);

    void AddTypesToAllTypes(IEnumerable<Type> types) => types.ForEach(type => _allTypes[type] = type);

    void MapTypes(IEnumerable<Type> types)
        => Parallel.ForEach(types.Where(IsImplementation).ToArray(), AddTypeMappingsFor);

    void AddTypeMappingsFor(Type implementor)
        => implementor
            .AllBaseAndImplementingTypes()
            .ForEach(contract =>
            {
                var implementingTypes = GetImplementingTypesFor(contract);
                if (!implementingTypes.Contains(implementor))
                {
                    implementingTypes.Add(implementor);
                }
            });

    static bool IsImplementation(Type type)
    {
        var typeInfo = type.GetTypeInfo();
        return !typeInfo.IsInterface && !typeInfo.IsAbstract;
    }

    ConcurrentBag<Type> GetImplementingTypesFor(Type contract)
        => _contractsAndImplementors.GetOrAdd(contract, _ => new ConcurrentBag<Type>());
}
