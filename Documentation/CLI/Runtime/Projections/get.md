---
title: Get
description: Gets details of a specific Projection currently registered by Clients to the Runtime
weight: 20
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime projection get <identifier> <scope> [options]
```

### Arguments

| Argument       | Description                                                                                                             |
|----------------|-------------------------------------------------------------------------------------------------------------------------|
| `<identifier>` | The identifier or alias of the Projection to get details for                                                            |
| `<scope>`      | The scope of the Projection to get details for. Only required when the identifier or alias matches multiple projections |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only show Stream Processor status for the specified Tenant.                                              |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table.                                       |
| `--wide`                | If set, prints more details in table format for a wider output.                                          |
| `--help`                | Show help information.                                                                                   |
