// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rudimentary.given;

public record StringConcept(string x) : ConceptAs<string>(x)
{
    public static implicit operator StringConcept(string value) => new(value);
}