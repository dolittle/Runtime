---
title: Replay
description: Initiates reprocessing of events for a specific Event Handler currently registered by Clients to the Runtime
weight: 30
repository: https://github.com/dolittle/Runtime
---

## Replay all events

Initiates reprocessing of all events (from position 0 in the Event Handler Stream) for all Tenants.
If you want to only reprocess all events for a specific Tenant, use the `replay from 0` command.

```shell
dolittle runtime eventhandlers replay all <identifier> [options]
```

### Arguments

| Argument       | Description                                                             |
|----------------|-------------------------------------------------------------------------|
| `<identifier>` | The identifier of the Event Handler to replay. Format: id/alias[:scope] |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--help`                | Show help information.                                                                                   |


## Replay events from a specific position in the Event Handler Stream

Initiates reprocessing of events from the specified position (in the Event Handler Stream) for a specific Tenant.
This command will fail if the specified position is higher than the current position for the Event Handler, which would cause some events to be skipped.

```shell
dolittle runtime eventhandlers replay from <identifier> <position> [options]
```

### Arguments

| Argument       | Description                                                                                                               |
|----------------|---------------------------------------------------------------------------------------------------------------------------|
| `<identifier>` | The identifier of the Event Handler to replay. Format: id/alias[:scope]                                                   |
| `<position>`   | The position _in the Event Handler stream_ to star reprocessing events from. Cannot be greater than the current position. |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | The Tenant to replay events for. **Defaults to the _Development_ tenant.**                               |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--help`                | Show help information.                                                                                   |
