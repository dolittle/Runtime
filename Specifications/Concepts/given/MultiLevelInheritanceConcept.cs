// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specs.Concepts.given
{
    public class MultiLevelInheritanceConcept : InheritingFromLongConcept
    {
        public static implicit operator MultiLevelInheritanceConcept(long value)
        {
            return new MultiLevelInheritanceConcept { Value = value };
        }
    }
}