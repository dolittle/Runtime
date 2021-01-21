// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Concepts.Serialization.Protobuf.Specs
{
    public class ConceptAsGuid : ConceptAs<Guid>
    {
        public static implicit operator ConceptAsGuid(Guid guid)
        {
            return new ConceptAsGuid() { Value = guid };
        }
    }
}