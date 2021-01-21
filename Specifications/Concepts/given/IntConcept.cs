// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Specs.Concepts.given
{
    public class IntConcept : ConceptAs<int>
    {
        public IntConcept()
        {
        }

        public IntConcept(int value)
        {
            Value = value;
        }

        public static implicit operator IntConcept(int value)
        {
            return new IntConcept { Value = value };
        }
    }
}