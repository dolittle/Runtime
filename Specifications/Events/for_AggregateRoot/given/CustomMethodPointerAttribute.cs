// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot.given
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class CustomMethodPointerAttribute : System.Attribute
    {
        public CustomMethodPointerAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
    }
}