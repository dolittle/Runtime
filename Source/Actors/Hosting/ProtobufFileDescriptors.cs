// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dolittle.Runtime.Actors.Hosting;

[Singleton]
public class ProtobufFileDescriptors : IProtobufFileDescriptors
{
    readonly IEnumerable<GrainAndActor> _grainsAndActors;
    
    IEnumerable<FileDescriptor> _all;

    public ProtobufFileDescriptors(IEnumerable<GrainAndActor> grainsAndActors)
    {
        _grainsAndActors = grainsAndActors;
    }

    public IEnumerable<FileDescriptor> All => _all ??= FindFileDescriptors();
    
    IEnumerable<FileDescriptor> FindFileDescriptors()
    {
        var fileDescriptors = new HashSet<FileDescriptor>();
        var rpcMethods = _grainsAndActors
            .Select(_ => GetAllRpcMethods(_.Grain))
            .SelectMany(_ => _);

        foreach (var method in rpcMethods)
        {
            foreach (var fileDescriptor in GetFileDescriptorsFromParameters(method.GetParameters()))
            {
                fileDescriptors.Add(fileDescriptor);
            }
            
            var returnType = method.ReturnType;
            if (returnType.IsGenericType)
            {
                returnType = returnType.GetGenericArguments().First();
            }
            if (IsMessageType(returnType))
            {
                fileDescriptors.Add(GetFileDescriptorFromMessageType(returnType));
            }
        }
        return fileDescriptors;
    }
    
    static IEnumerable<MethodInfo> GetAllRpcMethods(Type type)
        => type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(_ => IsOverriddenMethod(_) && IsDeclaredLocally(_, type));

    static IEnumerable<FileDescriptor> GetFileDescriptorsFromParameters(IEnumerable<ParameterInfo> parameters)
    {
        foreach (var parameter in parameters)
        {
            var parameterType = parameter.ParameterType;
            if (IsMessageType(parameterType))
            {
                yield return GetFileDescriptorFromMessageType(parameterType);
            }
        }
    }
    
    static FileDescriptor GetFileDescriptorFromMessageType(Type type)
    {
        var messageDescriptor = type
            .GetProperty(nameof(IMessage.Descriptor), BindingFlags.Static | BindingFlags.Public)!
            .GetValue(null, null) as MessageDescriptor;
        return messageDescriptor!.File;
    }
    
    static bool IsOverriddenMethod(MethodInfo method) => method.IsVirtual && !method.IsFinal && !method.Equals(method.GetBaseDefinition());
    
    static bool IsDeclaredLocally(MethodInfo method, Type grainType) => method.DeclaringType?.Equals(grainType) ?? false;

    static bool IsMessageType(Type type) => typeof(IMessage).IsAssignableFrom(type);
}
