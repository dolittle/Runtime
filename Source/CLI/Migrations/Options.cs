// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    /// <summary>
    /// The options used for the "dolittle migrate ..." commands.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// The Runtime version to migrate data stores from.
        /// </summary>
        [Required]
        [Option("--from <VERSION>", "The Runtime version to migrate from.", CommandOptionType.SingleValue)]
        public Version From { get; init; }

        /// <summary>
        /// The Runtime version to migrate data stores to.
        /// </summary>
        [Required]
        [Option("--to <VERSION>", "The Runtime version to migrate to.", CommandOptionType.SingleValue)]
        public Version To { get; init; }/// <summary>
        
        /// The Runtime resources configuration file name.
        /// </summary>
        [Option("--resources <RESOURCES>", "The Runtime resources configuration file name.", CommandOptionType.SingleValue)]
        public string ResourcesConfigName { get; init; }
    }
}