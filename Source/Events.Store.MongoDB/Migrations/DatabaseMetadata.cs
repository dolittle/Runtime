// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations;

public class DatabaseMetadata
{
    [BsonId] public string Id { get; set; } = "database";
    public List<Migration> Migrations { get; set; } = new();
    public required string CurrentVersion { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }

    public class Migration
    {
        public required string Version { get; set; }
        public required DateTimeOffset Timestamp { get; set; }
    }
}
