// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Re-enqueuing boot procedure called '{ProcedureType}'")]
    internal static partial void ReEnqueuingBootProcedure(ILogger logger, string procedureType);
    
    [LoggerMessage(0, LogLevel.Debug, "Performing boot procedure called '{ProcedureType}'")]
    internal static partial void PerformingBootProcedure(ILogger logger, string procedureType);
    
    [LoggerMessage(0, LogLevel.Debug, "Starting to perform {NumberOfBootProcedures} boot procedures")]
    internal static partial void StartPerformingBootProcedures(ILogger logger, int numberOfBootProcedures);
    
    [LoggerMessage(0, LogLevel.Trace, "Bootstrapper start all procedures")]
    internal static partial void StartAllBootProcedures(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Debug, "<********* BOOTSTAGE : {BootStage}{Suffix} *********>")]
    internal static partial void BootStage(ILogger logger, BootStage bootStage, string suffix);
}
