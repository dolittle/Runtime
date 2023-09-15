// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.IO;

namespace Integration.Tests.Events.Processing.given;

static class event_content_serializer
{
    public static readonly JsonWriterSettings json_settings = new()
    {
        OutputMode = JsonOutputMode.CanonicalExtendedJson,
        Indent = false,
    }; 
}