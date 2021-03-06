// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace Dolittle.Runtime.Reflection.Specs.for_TypeExtensions.for_IsEnumerable
{
    public class MyEnumerable : IEnumerable<ComplexType>
    {
        IEnumerable<ComplexType> _list = new List<ComplexType>();

        public IEnumerator<ComplexType> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}