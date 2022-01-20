// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Dolittle.Runtime.Collections;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Types;

/// <summary>
/// Represents an implementation of <see cref="IContractToImplementorsSerializer"/>.
/// </summary>
public class ContractToImplementorsSerializer : IContractToImplementorsSerializer
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractToImplementorsSerializer"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ContractToImplementorsSerializer(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public string SerializeMap(IDictionary<Type, IEnumerable<Type>> map)
    {
        var builder = new StringBuilder();
        map.ForEach(keyValue =>
        {
            var (key, value) = keyValue;
            var contract = GetAssemblyQualifiedNameFor(key);
            builder.Append(CultureInfo.InvariantCulture, $"{contract}:");
            var first = true;
            value.ForEach(implementor =>
            {
                if (!first)
                {
                    builder.Append(";");
                }
                first = false;
                builder.Append(GetAssemblyQualifiedNameFor(implementor));
            });
            builder.Append("\n");
        });

        return builder.ToString();
    }

    /// <inheritdoc/>
    public string SerializeTypes(IEnumerable<Type> types)
    {
        var builder = new StringBuilder();
        types.ForEach(_ =>
        {
            builder.Append(GetAssemblyQualifiedNameFor(_));
            builder.Append("\n");
        });
        return builder.ToString();
    }

    /// <inheritdoc/>
    public IDictionary<Type, IEnumerable<Type>> DeserializeMap(string serializedMap)
    {
        var map = new Dictionary<Type, IEnumerable<Type>>();

        var lines = serializedMap.Split('\n');
        var contractsCount = 0;
        var implementorsCount = 0;
        lines.ForEach(line =>
        {
            var keyValue = line.Split(':');
            var contract = Type.GetType(keyValue[0]);

            if (contract != null)
            {
                var implementors = keyValue[1]
                    .Split(';')
                    .Select(Type.GetType)
                    .Where(_ => _ != null)
                    .ToArray();
                if (implementors.Length > 0)
                {
                    map[contract] = implementors;
                    contractsCount++;
                    implementorsCount += implementors.Length;
                }
                else
                {
                    Log.NoImplementationsOfContract(_logger, keyValue[0]);
                }
            }
            else
            {
                Log.CannotFindContractType(_logger, keyValue[0], line);
            }
        });

        Log.UsingContractsMappedToImplementors(_logger, contractsCount, implementorsCount);

        return map;
    }

    /// <inheritdoc/>
    public IEnumerable<Type> DeserializeTypes(string serializedTypes)
    {
        var lines = serializedTypes.Split('\n');
        return lines.Select(Type.GetType)
            .Where(_ => _ != null)
            .ToArray();
    }

    static string GetAssemblyQualifiedNameFor(Type type)
    {
        var name = type.AssemblyQualifiedName;
        if (string.IsNullOrEmpty(name))
        {
            name = type.IsGenericType ? $"{type.Namespace}.{type.Name}, {type.Assembly.GetName().FullName}" : $"{type}, {type.Assembly.GetName().FullName}";
        }

        return name;
    }
}
