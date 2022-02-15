// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using Dolittle.Runtime.ApplicationModel;
// using Dolittle.Runtime.DependencyInversion;
// using Dolittle.Runtime.Execution;
//
// namespace Dolittle.Runtime.Applications.Configuration;
// TODO: 
// /// <summary>
// /// Binds up the bindings related to the running application. The<see cref="ApplicationId"/>, the <see cref="MicroserviceId"/> and the <see cref="Environment"/>.
// /// </summary>
// public class ApplicationBindings : ICanProvideBindings
// {
//     readonly BoundedContextConfiguration _boundedContextConfiguration;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="ApplicationBindings"/> class.
//     /// </summary>
//     /// <param name="boundedContextConfiguration"><see cref="BoundedContextConfiguration">Configuration</see> for the bounded context.</param>
//     public ApplicationBindings(BoundedContextConfiguration boundedContextConfiguration)
//     {
//         _boundedContextConfiguration = boundedContextConfiguration;
//     }
//
//     /// <inheritdoc/>
//     public void Provide(IBindingProviderBuilder builder)
//     {
//         builder.Bind<ApplicationId>().To(() => _boundedContextConfiguration.Application);
//         builder.Bind<MicroserviceId>().To(() => _boundedContextConfiguration.BoundedContext);
//     }
// }
