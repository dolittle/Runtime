// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Specs.Concepts.for_StringExtensions
{
    public class ConceptAsGuid : ConceptAs<Guid>
    {
        public static implicit operator ConceptAsGuid(Guid id)
        {
            return new ConceptAsGuid() { Value = id };
        }
    }
}