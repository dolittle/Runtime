// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor.given
{
    class failed_handling_result : IHandlerResult
    {
        public ProcessingResultValue Value => ProcessingResultValue.Failed;
    }
}