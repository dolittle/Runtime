// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    public class ConceptWithNoDefaultConstructor : ConceptAs<long>
    {
        public ConceptWithNoDefaultConstructor(long value) => Value = value;

        public ConceptWithNoDefaultConstructor(int value) => Value = value;
    }
}
