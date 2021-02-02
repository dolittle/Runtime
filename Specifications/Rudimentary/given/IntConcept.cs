// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



namespace Dolittle.Runtime.Rudimentary.given
{
    public record IntConcept(int value) : ConceptAs<int>(value)
    {
        public static implicit operator IntConcept(int value) => new(value);
    }
}