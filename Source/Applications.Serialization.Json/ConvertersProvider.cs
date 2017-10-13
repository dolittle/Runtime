/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Newtonsoft.Json;
using doLittle.Serialization.Json;
using doLittle.Runtime.Applications;

namespace doLittle.Runtime.Applications.Serialization.Json
{
    /// <summary>
    /// Provides converters related to concepts for Json serialization purposes
    /// </summary>
    public class ConvertersProvider : ICanProvideConverters
    {
        readonly IApplicationResourceIdentifierConverter _applicationResourceIdentifierConverter;

        /// <summary>
        /// Initializes a new instance 
        /// </summary>
        /// <param name="applicationResourceIdentifierConverter"><see cref="IApplicationResourceIdentifierConverter"/> for converting to and from <see cref="IApplicationResourceIdentifier"/></param>
        public ConvertersProvider(IApplicationResourceIdentifierConverter applicationResourceIdentifierConverter)
        {
            _applicationResourceIdentifierConverter = applicationResourceIdentifierConverter;
        }

        /// <inheritdoc/>
        public IEnumerable<JsonConverter> Provide()
        {
            return new JsonConverter[] {
                new ApplicationResourceIdentifierJsonConverter(_applicationResourceIdentifierConverter)
            };
        }
    }
}