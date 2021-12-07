// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Defines the base protocol for filters
/// </summary>
/// <typeparam name="TFilterClientMessage"></typeparam>
/// <typeparam name="TFilterRegistrationRequest"></typeparam>
/// <typeparam name="TFilterResponse"></typeparam>
/// <typeparam name="TRuntimeRegistrationArguments"></typeparam>
public interface IFiltersProtocol<TFilterClientMessage, TFilterRegistrationRequest, TFilterResponse, TRuntimeRegistrationArguments> : IReverseCallServiceProtocol<TFilterClientMessage, FilterRuntimeToClientMessage, TFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, TFilterResponse, TRuntimeRegistrationArguments>
    where TFilterClientMessage : IMessage, new()
    where TFilterRegistrationRequest : class
    where TFilterResponse : class
    where TRuntimeRegistrationArguments : class
{
}