// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Concepts.Serialization.Protobuf.Specs
{
    public class ConceptAsString : ConceptAs<string>
    {
        public static implicit operator ConceptAsString(string value)
        {
            return new ConceptAsString { Value = value };
        }
    }
}