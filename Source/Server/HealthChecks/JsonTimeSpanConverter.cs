// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dolittle.Runtime.Server.HealthChecks
{
    public class JsonTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(TimeSpan);

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());

    }
}

