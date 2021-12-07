// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Serialization.Json.Specs.for_Serializer;

public record Complex(Guid Concept, Immutable Immutable, int Primitive, Dictionary<string, object> Content);