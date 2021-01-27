// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Dolittle.Runtime.Rudimentary.given
{
    public record LongConcept(long value) : ConceptAs<long>(value)
    {
        public static implicit operator LongConcept(long value) => new(value);
    }
}