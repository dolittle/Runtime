// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions.given
{
    public class no_message_descriptions
    {
        protected static MessageDescriptions message_descriptions;

        Establish context = () => message_descriptions = new MessageDescriptions();
    }
}