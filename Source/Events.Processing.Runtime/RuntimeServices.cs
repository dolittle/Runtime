// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

// using System.Collections.Generic;
// using Dolittle.Runtime.Heads;
// using Dolittle.Services;

// namespace Dolittle.Runtime.Events.Runtime
// {
//     /// <summary>
//     /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
//     /// runtime service implementations for Heads.
//     /// </summary>
//     public class RuntimeServices : ICanBindRuntimeServices
//     {
//         readonly FiltersService _filtersService;

//         /// <summary>
//         /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
//         /// </summary>
//         /// <param name="filtersService">The <see cref="FiltersService"/>.</param>
//         public RuntimeServices(FiltersService filtersService)
//         {
//             _filtersService = filtersService;
//         }

//         /// <inheritdoc/>
//         public ServiceAspect Aspect => "Events.Processing";

//         /// <inheritdoc/>
//         public IEnumerable<Service> BindServices()
//         {
//             return new Service[]
//             {
//                 new Service(_filtersService, Dolittle.Events.Processing.Runtime.Filters.BindService(_filtersService), Dolittle.Events.Processing.Runtime.Filters.Descriptor)
//             };
//         }
//     }
// }