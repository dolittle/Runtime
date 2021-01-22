// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    public class ClassWithNestedConcepts : Value<ClassWithNestedConcepts>
    {
        public ConceptAsGuid GuidConcept { get; set; }

        public ConceptAsString StringConcept { get; set; }

        public ConceptAsLong LongConcept { get; set; }

        public ClassWithConcepts NestedConcepts { get; set; }
    }
}