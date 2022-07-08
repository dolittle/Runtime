// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Integration.Shared;

public record RunningRuntime(IHost Host, IEnumerable<TenantId> ConfiguredTenants, IMongoClient MongoClient, IEnumerable<string> Databases);
