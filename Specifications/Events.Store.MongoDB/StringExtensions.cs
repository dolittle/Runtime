// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class StringExtensions
    {
        public static string FormatJson(this string json)
            => JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement);
    }
}
