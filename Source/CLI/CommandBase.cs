// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.CLI.Options;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;
using ConsoleTables;
using Dolittle.Runtime.CLI.Configuration.Files;
using Newtonsoft.Json;

namespace Dolittle.Runtime.CLI;

/// <summary>
/// A shared command base for the "dolittle" commands that provides shared arguments.
/// </summary>
public abstract class CommandBase
{
    readonly ISerializer _jsonSerializer;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    protected CommandBase(ISerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }
        
    [Option("-o|--output", CommandOptionType.SingleValue, Description = "The output type.")]
    protected OutputType Output { get; } = OutputType.Table;

    [Option("-w|--wide", CommandOptionType.NoValue, Description = "Whether the table output is wide or not.")]
    protected bool Wide { get; }

    protected Task WriteOutput<T>(CommandLineApplication cli, T obj)
        => cli.Out.WriteAsync(CreateOutput(new [] {obj}, true));
        
    protected Task WriteOutput<T>(CommandLineApplication cli, IEnumerable<T> objects)
        => cli.Out.WriteAsync(CreateOutput(objects.ToArray(), false));
        
    string CreateOutput<T>(T[] obj, bool singular)
    {
        var output = Output switch
        {
            OutputType.Table => ConsoleTable.From(obj).ToMinimalString(),
            OutputType.Json => _jsonSerializer.ToJson(
                !singular ? obj : obj[0],
                SerializationOptions.Custom(SerializationOptionsFlags.None, callback: _ =>
                {
                    _.Formatting = Formatting.Indented;
                })),
            _ => ""
        };
        output += "\n";
        return output;
    }
}