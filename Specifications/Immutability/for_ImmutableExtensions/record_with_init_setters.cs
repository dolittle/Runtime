// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions;

public record record_with_init_setters
{
    public int property_with_getter { get; }
    public int property_with_init_setter { get; init; }
}