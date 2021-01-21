// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs
{
    public class ConceptAsString : ConceptAs<string>
    {
        public static implicit operator ConceptAsString(string value)
        {
            return new ConceptAsString { Value = value };
        }
    }
}