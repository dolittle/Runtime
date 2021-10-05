// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    public class Options
    {
        [Required]
        [Option("--from <VERSION>", Description = "The Runtime version to migrate from.")]
        public string From { get; init; }

        [Required]
        [Option("--to <VERSION>", Description = "The Runtime version to migrate to.")]
        public string To { get; init; }
    }
}