// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Collections.Generic;
// using Dolittle.Runtime.Serialization.Json;
// using Newtonsoft.Json;
//
// namespace Dolittle.Runtime.CLI.Serialization;
//
// /// <summary>
// /// Represents an implementation of <see cref="ICanProvideConverters"/> that provides a static set of converters.
// /// </summary>
// public class StaticConverterProvider : ICanProvideConverters
// {
//     readonly JsonConverter[] _converters;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="StaticConverterProvider"/> class.
//     /// </summary>
//     /// <param name="converters">The converters that will be provided.</param>
//     public StaticConverterProvider(params JsonConverter[] converters)
//     {
//         _converters = converters;
//     }
//
//     /// <inheritdoc />
//     public IEnumerable<JsonConverter> Provide() => _converters;
// }
