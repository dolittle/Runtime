// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Data.HashFunction;
using System.Data.HashFunction.CityHash;
using System.Text;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertProjectionKeysToEventSourceIds"/>.
    /// </summary>
    public class ProjectionKeyToEventSourceIdConverter : IConvertProjectionKeysToEventSourceIds
    {
        const int _eventSourceIdBitLength = 128;

        readonly Encoding _encoding;
        readonly ICityHash _hasher;

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionKeyToEventSourceIdConverter" /> class.
        /// </summary>
        public ProjectionKeyToEventSourceIdConverter()
        {
            _encoding = Encoding.Unicode;
            _hasher = CityHashFactory.Instance.Create(new CityHashConfig { HashSizeInBits = _eventSourceIdBitLength });
        }

        /// <inheritdoc/>
        public EventSourceId GetEventSourceIdFor(ProjectionKey key)
        {
            var bytes = _encoding.GetBytes(key.Value);
            var hash = _hasher.ComputeHash(bytes);

            ThrowIfComputedHashIsNotCorrectLength(hash);

            return new Guid(hash.Hash);
        }

        void ThrowIfComputedHashIsNotCorrectLength(IHashValue hash)
        {
            if (hash.BitLength != _eventSourceIdBitLength)
            {
                throw new ComputedHashIsNotOfCorrectLength(hash.BitLength, _eventSourceIdBitLength);
            }
        }
    }
}
