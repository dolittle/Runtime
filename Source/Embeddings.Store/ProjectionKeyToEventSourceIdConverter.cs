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
        readonly Encoding _encoding;
        readonly ICityHash _hasher;

        public ProjectionKeyToEventSourceIdConverter()
        {
            _encoding = Encoding.Unicode;
            _hasher = CityHashFactory.Instance.Create(new CityHashConfig { HashSizeInBits = 128 });
        }

        /// <inheritdoc/>
        public EventSourceId GetEventSourceIdFor(ProjectionKey key)
        {
            var bytes = _encoding.GetBytes(key.Value);
            var hash = _hasher.ComputeHash(bytes);

            ThrowIfHashIsNot128BitsLong(hash);

            return new Guid(hash.Hash);
        }

        void ThrowIfHashIsNot128BitsLong(IHashValue hash)
        {
            if (hash.BitLength != 128)
            {
                throw new ComputedHashIsNotOfCorrectLength(hash.BitLength);
            }
        }
    }
}
