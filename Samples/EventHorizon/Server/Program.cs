// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Server
{
    static class Program
    {
        static async Task Main()
        {
            await Bootloader.Start().ConfigureAwait(false);
        }
    }
}
