// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder.given
{
    public class an_empty_boot_stage_builder
    {
        protected static BootStageBuilder builder;

        Establish context = () => builder = new BootStageBuilder();
    }
}