// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Concepts.Serialization.Json
{
    /// <summary>
    /// Provides converters related to concepts for Json serialization purposes.
    /// </summary>
    public class ConverterProvider : ICanProvideConverters
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterProvider"/> class.
        /// </summary>
        /// <param name="logger">A logger.</param>
        public ConverterProvider(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<JsonConverter> Provide()
        {
            return new JsonConverter[]
            {
                new ConceptConverter(),
                new ConceptDictionaryConverter(_logger)
            };
        }
    }
}