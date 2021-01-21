// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    public class Immutable : Value<Immutable>
    {
        public Immutable(ConceptAsGuid guid, ConceptAsString label, ConceptAsLong number)
        {
            Label = label;
            Guid = guid;
            Number = number;
        }

        public ConceptAsString Label { get; }

        public ConceptAsGuid Guid { get; }

        public ConceptAsLong Number { get; set; }
    }
}