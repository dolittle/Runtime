// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Serialization.Json.Specs.for_Serializer;

public class TypeThatCannotBeCreated
{
    public TypeThatCannotBeCreated(string myProp, string other)
    {
        MyProperty = myProp;
        MyOtherProperty = other;
    }

    public string MyProperty { get; }

    public string MyOtherProperty { get; }
}