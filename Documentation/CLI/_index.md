---
title: Dolittle CLI
description: The Dolittle CLI tool command reference 
weight: 10
repository: https://github.com/dolittle/Runtime
---

This section helps you learn about how to use the Dolittle CLI tool. If you're new to the CLI, jump down to the [how to install section]({{< ref "#how-to-install" >}}) to get started.

## Command overview

| Syntax                                  | Description                                         |                                                       |
|-----------------------------------------|-----------------------------------------------------|-------------------------------------------------------|
| `dolittle runtime aggregates list`      | List all running Aggregate Roots                    | [Details]({{< ref "Runtime/Aggregates/list" >}})      |
| `dolittle runtime aggregates get`       | Get details about a running Aggregate Root          | [Details]({{< ref "Runtime/Aggregates/get" >}})       |
| `dolittle runtime aggregates events`    | Get committed events for an Aggregate Root Instance | [Details]({{< ref "Runtime/Aggregates/events" >}})    |
| `dolittle runtime eventhandlers list`   | List all running Event Handlers                     | [Details]({{< ref "Runtime/EventHandlers/list" >}})   |
| `dolittle runtime eventhandlers get`    | Get details about a running Event Handler           | [Details]({{< ref "Runtime/EventHandlers/get" >}})    |
| `dolittle runtime eventhandlers replay` | Replay events for a running Event Handler           | [Details]({{< ref "Runtime/EventHandlers/replay" >}}) |
| `dolittle runtime eventtypes list`      | List all registered Event Types                     | [Details]({{< ref "Runtime/EventTypes/list" >}})      |
| `dolittle runtime projections list`     | List all running Projections                        | [Details]({{< ref "Runtime/Projections/list" >}})     |
| `dolittle runtime projections get`      | Get details about a running Projection              | [Details]({{< ref "Runtime/Projections/get" >}})      |
| `dolittle runtime projections replay`   | Replay a running Projection                         | [Details]({{< ref "Runtime/Projections/replay" >}})   |

## How to install

There are two ways to install the Dolittle CLI tool, directly as a binary or using the `dotnet tool` command if you're using .NET.

### Installing as a .NET tool

To install the tool globally on your machine, run:
```shell
dotnet tool install --global Dolittle.Runtime.CLI
```

This should make the `dolittle` command anywhere. You might have to modify your `PATH` environment variable to make it work, and the .NET installer should guide you in how to do this. If it doesn't, you can have a look at the [dotnet tool install documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) for more help.

### Installing as a binary

To install the tool manually, head over to the [Runtime latest release page on GitHub](https://github.com/dolittle/Runtime/releases/latest), expand the "Assets" section at the bottom of the release, and download the binary for your setup.
Next you'll have to place this file somewhere in your `PATH` to make it available as a command, on a *nix-like system, `/usr/local/bin` is usually a nice place, in the process of moving it we also recommend that you rename it to just `dolittle`.
Lastly you will need to make the file executable by running `chomd a+x /usr/local/bin/dolittle` and you should be all set.

{{< alert title="Updates" color="warning" >}}
The Dolittle CLI tool does currently not check for new versions by itself.
So you will need to either download a new binary from the releases page and replace the current one, or run `dotnet tool update --global Dolittle.Runtime.CLI` to get a fresh version with new features.
{{< /alert >}}

### Subcommands
