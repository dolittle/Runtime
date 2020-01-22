// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a <see cref="NullFreeDictionary{TKey, TValue}" /> for <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorDictionary : NullFreeDictionary<StreamProcessorKey, StreamProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorDictionary"/> class.
        /// </summary>
        public StreamProcessorDictionary()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorDictionary"/> class.
        /// </summary>
        /// <param name="otherDictionary">The other dictionary.</param>
        public StreamProcessorDictionary(IDictionary<StreamProcessorKey, StreamProcessor> otherDictionary)
            : base(otherDictionary)
        {
        }
    }
}