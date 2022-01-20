// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.given;

public class an_event_content_converter
{
    protected static Mock<IEventContentConverter> event_content_converter;
    protected static BsonDocument bson_returned_by_event_converter;
    protected static string json_returned_by_event_converter;

    Establish context = () =>
    {
        event_content_converter = new Mock<IEventContentConverter>();

        bson_returned_by_event_converter = new BsonDocument();
        event_content_converter
            .Setup(_ => _.ToBson(It.IsAny<string>()))
            .Returns(bson_returned_by_event_converter);

        json_returned_by_event_converter = "{}";
        event_content_converter
            .Setup(_ => _.ToJson(It.IsAny<BsonDocument>()))
            .Returns(json_returned_by_event_converter);
    };
}