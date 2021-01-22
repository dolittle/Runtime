// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Dolittle.Runtime.Reflection
{
    /// <summary>
    /// Represents an implementation of <see cref="IProxying"/>.
    /// </summary>
    public class Proxying : IProxying
    {
        const string DynamicAssemblyName = "Dynamic Assembly";
        const string DynamicModuleName = "Dynamic Module";

        static readonly AssemblyBuilder DynamicAssembly;
        static readonly ModuleBuilder DynamicModule;

        static Proxying()
        {
            var dynamicAssemblyName = CreateUniqueName(DynamicAssemblyName);
            var dynamicModuleName = CreateUniqueName(DynamicModuleName);
            var assemblyName = new AssemblyName(dynamicAssemblyName);
            DynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            DynamicModule = DynamicAssembly.DefineDynamicModule(dynamicModuleName);
        }

        /// <inheritdoc/>
        public Type BuildInterfaceWithPropertiesFrom(Type type)
        {
            var typeBuilder = DefineInterface(type);

            foreach (var property in type.GetTypeInfo().GetProperties())
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, Array.Empty<Type>());
                var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, property.PropertyType, Array.Empty<Type>());
                propertyBuilder.SetGetMethod(getMethodBuilder);
                var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, property.PropertyType, new[] { property.PropertyType });
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            var interfaceForType = typeBuilder.CreateTypeInfo().AsType();
            return interfaceForType;
        }

        /// <inheritdoc/>
        public Type BuildClassWithPropertiesFrom(Type type)
        {
            var typeBuilder = DefineClass(type);

            foreach (var property in type.GetTypeInfo().GetProperties())
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, Array.Empty<Type>());
                var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, property.PropertyType, Array.Empty<Type>());
                propertyBuilder.SetGetMethod(getMethodBuilder);
                var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, property.PropertyType, new[] { property.PropertyType });
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            var classForType = typeBuilder.CreateTypeInfo().AsType();
            return classForType;
        }

        static string CreateUniqueName(string prefix)
        {
            var uid = Guid.NewGuid().ToString();
            uid = uid.Replace('-', '_');
            return $"{prefix}{uid}";
        }

        static TypeBuilder DefineInterface(Type type)
        {
            var name = CreateUniqueName(type.Name);
            return DynamicModule.DefineType(name, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Serializable);
        }

        static TypeBuilder DefineClass(Type type)
        {
            var name = CreateUniqueName(type.Name);
            return DynamicModule.DefineType(name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
        }
    }
}
