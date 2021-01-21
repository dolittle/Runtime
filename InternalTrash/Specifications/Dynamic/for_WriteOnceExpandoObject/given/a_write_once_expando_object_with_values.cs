// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Dynamic.Specs.for_WriteOnceExpandoObject.given
{
    public class a_write_once_expando_object_with_values
    {
        protected const string IntegerKey = "Integer";
        protected static dynamic values;

        Establish context = () => values = new WriteOnceExpandoObject(d => d[IntegerKey] = 5);
    }
}
