// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils;

namespace CLI
{
    [Command(Name = "dolittle", Description = "The Dolittle CLI tool")]
    [Subcommand(typeof(Migrate))]
    class Program
    {
        static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        public void OnExecute(CommandLineApplication cli)
            => cli.ShowHelp();
    }
}
