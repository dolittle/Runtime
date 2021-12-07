// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Types;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Error, "TypeFeed failure for assembly {AssemblyName} : {LoaderExceptionSource} {LoaderExceptionMessage}")]
    internal static partial void TypeFeedFailureForAssembly(ILogger logger, string assemblyName, string loaderExceptionSource, string loaderExceptionMessage);

    [LoggerMessage(0, LogLevel.Debug, "No implementations of '{Contract}'")]
    internal static partial void NoImplementationsOfContract(ILogger logger, string contract);

    [LoggerMessage(0, LogLevel.Debug, "Can't find contract type '{Contract}' - {Line}")]
    internal static partial void CannotFindContractType(ILogger logger, string contract, string line);

    [LoggerMessage(0, LogLevel.Trace, "Using {ContractsCount} contracts mapped to {ImplementorsCount} implementors in total")]
    internal static partial void UsingContractsMappedToImplementors(ILogger logger, int contractsCount, int implementorsCount);
}
