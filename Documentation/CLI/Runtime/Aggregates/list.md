---
title: List
description: Lists all the Aggregate Roots currently registered by Clients to the Runtime
weight: 10
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime aggregates list [options]
```

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only show Aggregate Root information for the specified Tenant.                                           |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table.                                       |
| `--wide`                | If set, prints more details in table format for a wider output.                                          |
| `--help`                | Show help information.                                                                                   |
