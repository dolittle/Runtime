---
title: Get
description: Gets details of a specific Aggregate Root currently registered by Clients to the Runtime
weight: 20
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime aggregates get <identifier> [options]
```

### Arguments

| Argument        | Description                                                   |
|-----------------|---------------------------------------------------------------|
| `<identifier>`  | The id or the alias of the Aggregate Root to get details for. |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only show Aggregate Root information for the specified Tenant.                                           |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table.                                       |
| `--wide`                | If set, prints more details in table format for a wider output.                                          |
| `--help`                | Show help information.                                                                                   |
