// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specs.Concepts.given
{
    public class InheritingFromLongConcept : LongConcept
    {
        public static implicit operator InheritingFromLongConcept(long value)
        {
            return new InheritingFromLongConcept { Value = value };
        }
    }
}