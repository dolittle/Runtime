// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines the signature of a ScopedStreamProcessorFailedToProcessEvent event.
/// </summary>
public delegate void ScopedStreamProcessorFailedToProcessEvent(StreamEvent @event, TimeSpan processingTime);
