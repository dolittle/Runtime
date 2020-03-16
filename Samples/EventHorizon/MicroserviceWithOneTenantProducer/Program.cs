// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Samples.MicroserviceWithOneTenantProducer
{
    static class Program
    {
        static async Task Main()
        {
            await Server.Bootloader.Start().ConfigureAwait(false);
        }
    }
}
