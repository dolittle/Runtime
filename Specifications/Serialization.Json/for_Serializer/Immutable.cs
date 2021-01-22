// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Serialization.Json.Specs.for_Serializer
{
    public class Immutable : Value<Immutable>
    {
        public Immutable(Guid guid, string label)
        {
            Label = label;
            Guid = guid;
        }

        public string Label { get; }

        public Guid Guid { get; }
    }
}