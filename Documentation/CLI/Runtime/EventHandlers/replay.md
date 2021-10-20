---
title: Replay
description: Initiates reprocessing of events for a specific Event Handler currently registered a Client to the Runtime
weight: 30
repository: https://github.com/dolittle/Runtime
---

## Replay all events

Initiates reprocessing of all events (from position 0 in the Event Handler Stream) for all Tenants.
If you want to only reprocess all events for a specific Tenant, use the `replay from 0` command.

```shell
dolittle runtime eventhandlers replay all [options]
```

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--id <id>`             | **Required:** The identifier of the Event Handler to replay.                                             |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--help`                | Show help information.                                                                                   |


## Replay events from a specific position in the Event Handler Stream

Initiates reprocessing of events from the specified position (in the Event Handler Stream) for a specific Tenant.
This command will fail if the specified position is higher than the current position for the Event Handler, which would cause some events to be skipped.

```shell
dolittle runtime eventhandlers replay from <position> [options]
```

### Arguments

| Argument     | Description                                                                                                               |
|--------------|---------------------------------------------------------------------------------------------------------------------------|
| `<position>` | The position _in the Event Handler stream_ to star reprocessing events from. Cannot be greater than the current position. |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--id <id>`             | **Required:** The identifier of the Event Handler to replay.                                             |
| `--tenant <id>`         | The Tenant to replay events for. **Defaults to the _Development_ tenant.**                               |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--help`                | Show help information.                                                                                   |