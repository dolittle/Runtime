// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions
{
    public interface IConvertFromOldToNew<in TOld, out TNew>
    {
        TNew Convert(TOld old);
    }

}