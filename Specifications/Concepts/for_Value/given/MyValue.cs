// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Specs.Concepts.for_Value.given
{
    public class MyValue : Value<MyValue>
    {
        public string FirstStringValue { get; set; }

        public string SecondStringValue { get; set; }

        public int FirstIntValue { get; set; }

        public int SecondIntValue { get; set; }
    }
}