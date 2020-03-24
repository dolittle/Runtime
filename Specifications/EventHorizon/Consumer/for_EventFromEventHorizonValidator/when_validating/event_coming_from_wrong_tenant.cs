// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_EventFromEventHorizonValidator.when_validating
{
    public class event_coming_from_wrong_tenant
    {
        static EventHorizonEvent
        static EventFromEventHorizonValidator validator;

        Establish context = () => validator = new EventFromEventHorizonValidator();

        Because of = () => validator.Validate()
    }
}