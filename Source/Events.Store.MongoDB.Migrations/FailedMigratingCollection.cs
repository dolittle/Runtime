// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public class FailedMigratingCollection : Exception
    {
        public FailedMigratingCollection(string collectionName, Exception ex)
            : base($"Failed to perform migration on collection '{collectionName}'", ex)
        {
        }
    }
}