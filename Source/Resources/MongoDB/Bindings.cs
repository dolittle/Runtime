// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using Dolittle.Runtime.DependencyInversion;
//
// namespace Dolittle.Runtime.Resources.MongoDB;
//
// /// <summary>
// /// Represents <see cref="ICanProvideBindings">bindings</see> for the resources system.
// /// </summary>
// public class Bindings : ICanProvideBindings
// {
//     /// <inheritdoc />
//     public void Provide(IBindingProviderBuilder builder)
//     {
//         builder.Bind<IKnowTheConnectionString>().To<ConnectionStringFromResourceConfiguration>();
//         builder.Bind<ICanGetResourceForTenant>().To<ResourceForTenantGetter>();
//     }
// }
