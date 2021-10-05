// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils;

namespace CLI.Migrations
{
    public class Options
    {
        [Required]
        [Option("--from <VERSION>", "The Runtime version to migrate from.", CommandOptionType.SingleValue)]
        public Version From { get; init; }

        [Required]
        [Option("--to <VERSION>", "The Runtime version to migrate to.", CommandOptionType.SingleValue)]
        public Version To { get; init; }
    }
}