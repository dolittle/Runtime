// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions
{
    public interface IConvertFromOldToNew<TOld, TNew>
    {
        FilterDefinition<TOld> Filter { get; }
        TNew Convert(TOld old);
    }
}