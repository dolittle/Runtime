// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    public class ClassInheritingFromAReadOnlyDictionary : ReadOnlyDictionary<string, string>
    {
        public ClassInheritingFromAReadOnlyDictionary(IDictionary<string, string> dictionary)
            : base(dictionary)
        {
        }
    }
}