// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                .WithEventTypes(eventTypes => eventTypes.Register<DishPrepared>())
                .WithEventHandlers(builder => CreateEventHandlers(builder, 4))
                .Build();

            Console.WriteLine("Starting client");

            Task.Run(() => DoLoop(client));
            // DoOnce(client);
            client.Start().Wait();
        }

        static async Task DoLoop(Client client)
        {
            await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            var i = 0;
            while (true)
            {
                foreach (var tenant in GetTenants())
                {
                    Console.WriteLine($"Committing event {i} to tenant {tenant}");
                    var taco = new DishPrepared($"Taco {i} med lefse till {tenant}", "Mrs. Turbo Taco, with love");
                    await client.EventStore
                        .ForTenant(tenant)
                        .Commit(eventsBuilder =>
                            eventsBuilder
                                .CreateEvent(taco)
                                .FromEventSource("79354376-c7c4-4a0b-89f1-87092c6b2706")).ConfigureAwait(false);
                }
                i++;
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }
        static async Task DoOnce(Client client)
        {
            var i = 0;
            foreach (var tenant in GetTenants())
            {
                Console.WriteLine($"Committing event {i} to tenant {tenant}");
                var taco = new DishPrepared($"Taco {i} med lefse till {tenant}", "Mrs. Turbo Taco, with love");
                await client.EventStore
                    .ForTenant(tenant)
                    .Commit(eventsBuilder =>
                        eventsBuilder
                            .CreateEvent(taco)
                            .FromEventSource("79354376-c7c4-4a0b-89f1-87092c6b2706")).ConfigureAwait(false);
            }
        }

        static void CreateEventHandlers(EventHandlersBuilder builder, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                builder
                    .CreateEventHandler(Guid.NewGuid())
                    .Partitioned()
                    .Handle<DishPrepared>((@event, ctx) => System.Console.WriteLine($"Handled in Partitioned:  {i}. Event: {@event.Dish} with sequence number {ctx.SequenceNumber}"));
                builder
                    .CreateEventHandler(Guid.NewGuid())
                    .Unpartitioned()
                    .Handle<DishPrepared>((@event, ctx) => System.Console.WriteLine($"Handled in Unpartitioned:  {i}. Event: {@event.Dish} with sequence number {ctx.SequenceNumber}"));
            }
        }

        static IEnumerable<TenantId> GetTenants()
            => new List<TenantId>()
                {
                    "796fee66-4f96-4f05-a128-e41126da1304",
                    "5152f09b-2733-4f35-b249-b46c4a35a61a", 
                    "c80db8cd-068e-437f-b842-986abea24889",
                    "2c6ad8a9-4471-4f7b-a9c1-9c655aa9b254",
                    "9df1b4c0-0abd-4409-8f7a-27074d4881b3",
                    "80a43b85-d2a0-4895-9d26-01eff81e4383",
                    "1598c547-63a9-498c-ab62-c9130d661731",
                    "8e88013f-d261-4e11-b367-162595e295b6",
                    "25cddafb-2d97-4a3f-b880-27cc87c593d8",
                    "f94e947a-169b-4f5a-b41e-3590b419a61f",
                    "6afab21d-bc68-457d-854f-e574ba56f0ad",
                    "cca6b9bd-326e-466b-aa04-606dd45f9cba",
                    "eb46b2e0-a78f-4914-babe-a73dd9df53f6",
                    "58bbef4e-bfab-429a-82b3-9d0d0b733a0e",
                    "de9b7384-6bcf-4f58-ab7e-95e2761248aa",
                    "c24139a2-9f83-429f-a51f-7eade036441c",
                    "7998f48b-bcd4-4895-b5d5-5f115c90df51",
                    "0dac5e4e-4d9d-4d89-90e8-23562ea8f843",
                    "d61d349f-6310-48d5-b57b-1d8a706b4ee7",
                    "d7ee92f2-82aa-4e5c-ab43-66d1e0909e98",
                    "7df51dc4-265f-4ac1-bc7c-27e23bd65245",
                    "77882dee-dda5-4e84-a50e-fec3b2fcd807",
                    "2b7feea5-f6df-4c91-bdb9-74400f08d7f3",
                    "e027b43e-e647-47db-af16-af6956ef09d6",
                    "a2c1d060-2456-4eb8-9895-78e4119ae087",
                    "2209070a-b8be-4e65-a38b-ad406bfa2380",
                    "3ec76ced-cfc9-4e26-91a0-7ca6adb1f9cd",
                    "a1dbf654-d0b7-4230-8a8f-3d73b1c55637"
                };
    }
}
