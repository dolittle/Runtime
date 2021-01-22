// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Rules.Specs.for_Reason
{
    public class when_creating
    {
        static Guid id = new Guid("3847286b-b508-4738-8975-f08383999f5a");
        static string title = "Some Title";
        static string description = "Some description";
        static Reason reason;

        Because of = () => reason = Reason.Create(id, title, description);

        It should_have_the_id_set = () => reason.Id.Value.ShouldEqual(id);
        It should_have_title_set = () => reason.Title.ShouldEqual(title);
        It should_have_description_set = () => reason.Description.ShouldEqual(description);
    }
}
