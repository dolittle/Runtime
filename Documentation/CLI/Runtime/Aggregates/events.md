---
title: Events
description: Gets committed events for a specific Aggregate Root Instance that is currently registered by Clients to the Runtime
weight: 30
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime aggregates events <identifier> <eventsource> [options]
```

### Arguments

| Argument        | Description                                                                  |
|-----------------|------------------------------------------------------------------------------|
| `<identifier>`  | The id or the alias of the Aggregate Root to get details for.                |
| `<eventsource>` | The Event Source of the Aggregate Root Instance to get committed events for. |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only show committed events for the specified Tenant. Defaults to the development Tenant.                 |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table.                                       |
| `--wide`                | If set, prints more details in table format for a wider output.                                          |
| `--help`                | Show help information.                                                                                   |
