// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rudimentary.given
{
    public record MultiLevelInheritanceConcept(long value) : InheritingFromLongConcept(value)
    {
        public static implicit operator MultiLevelInheritanceConcept(long value) => new(value);
    }
}