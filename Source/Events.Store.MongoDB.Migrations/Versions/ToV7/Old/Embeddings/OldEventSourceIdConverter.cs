// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.CityHash;
using System.Text;
using Dolittle.Runtime.Projections.Store;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings
{
    public class OldEventSourceIdConverter : IConvertOldEventSourceId
    {
        const int EventSourceIdBitLength = 128;

        readonly Encoding _encoding;
        readonly ICityHash _hasher;

        public OldEventSourceIdConverter()
        {
            _encoding = Encoding.Unicode;
            _hasher = CityHashFactory.Instance.Create(new CityHashConfig { HashSizeInBits = EventSourceIdBitLength });
        }

        public EventSourceId Convert(Guid oldEventSource, IEnumerable<ProjectionKey> keys)
        {
            foreach (var key in keys)
            {
                if (GetEventSourceIdFor(key).Equals(oldEventSource))
                {
                    return key.Value;
                }
            }
            throw new CannotComputeEventSourceId(oldEventSource);
        }
        
        Guid GetEventSourceIdFor(ProjectionKey key)
        {
            var bytes = _encoding.GetBytes(key.Value);
            var hash = _hasher.ComputeHash(bytes);

            ThrowIfComputedHashIsNotCorrectLength(hash);

            return new Guid(hash.Hash);
        }

        void ThrowIfComputedHashIsNotCorrectLength(IHashValue hash)
        {
            if (hash.BitLength != EventSourceIdBitLength)
            {
                throw new ComputedHashIsNotOfCorrectLength(hash.BitLength, EventSourceIdBitLength);
            }
        }
    }
}