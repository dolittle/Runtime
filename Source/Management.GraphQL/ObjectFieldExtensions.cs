// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Management.GraphQL
{
    public static class ObjectFieldExtensions
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static IObjectTypeDescriptor AddSubObject<T>(this IObjectTypeDescriptor parent, string name)
        {
            parent.Field(name).Type(typeof(EventHandlers.EventHandlers)).Resolve(_ => ServiceProvider.GetService<T>());
            return parent;
        }
    }
}