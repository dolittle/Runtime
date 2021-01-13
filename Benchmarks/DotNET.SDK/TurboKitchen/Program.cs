// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Tenancy;

namespace Kitchen
{
    class Program
    {
        public static void Main()
        {
            var client = Client
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventHandlers(builder => CreateEventHandlers(builder, 9))
                .Build();
            
            System.Console.WriteLine("Starting client");
            client.Start().Wait();
        }
        static void CreateEventHandlers(EventHandlersBuilder builder, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                builder.CreateEventHandler(Guid.NewGuid()).Partitioned().Handle(Guid.NewGuid(), (@event, context) => {});
                builder.CreateEventHandler(Guid.NewGuid()).Unpartitioned().Handle(Guid.NewGuid(), (@event, ctx) => {});
            }
        }
    }
}
