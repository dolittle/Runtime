// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Samples.MicroserviceWithOneTenantSubscriber
{
    static class Program
    {
        static async Task Main()
        {
            await Dolittle.Runtime.Server.BootLoader.Start();
        }
    }
}
