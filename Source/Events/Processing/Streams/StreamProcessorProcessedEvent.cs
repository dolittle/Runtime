// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines the signature of a StreamProcessorProcessedEvent event.
/// </summary>
public delegate void StreamProcessorProcessedEvent(TenantId tenant, StreamEvent @event, TimeSpan processingTime);
