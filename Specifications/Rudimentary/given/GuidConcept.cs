// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary.given;

public record GuidConcept(Guid value) : ConceptAs<Guid>(value)
{
    public static implicit operator GuidConcept(Guid value) => new(value);
}