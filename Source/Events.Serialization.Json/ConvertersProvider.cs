/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;


namespace Dolittle.Runtime.Events.Serialization.Json
{
    /// <summary>
    /// Provides converters related to concepts for Json serialization purposes
    /// </summary>
    public class ConvertersProvider : ICanProvideConverters
    {
        /// <inheritdoc/>
        public IEnumerable<JsonConverter> Provide()
        {
            return new JsonConverter[] {
                new EventSourceVersionConverter()
            };
        }
    }
}
