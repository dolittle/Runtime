---
title: Get
description: Gets details of a specific Event Handler currently registered by Clients to the Runtime
weight: 20
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime eventhandlers get <identifier> [options]
```

### Arguments

| Argument       | Description                                                                      |
|----------------|----------------------------------------------------------------------------------|
| `<identifier>` | The identifier of the Event Handler to get details for. Format: id/alias[:scope] |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only show Stream Processor status for the specified Tenant.                                              |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table.                                       |
| `--wide`                | If set, prints more details in table format for a wider output.                                          |
| `--help`                | Show help information.                                                                                   |
