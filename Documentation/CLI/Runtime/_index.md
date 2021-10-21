---
title: Runtime
description: Commands related to management of a Runtime
weight: 10
repository: https://github.com/dolittle/Runtime
---

```shell
dolittle runtime [subcommand]
```

### Options

| Option                  | Description                                                        |
|-------------------------|--------------------------------------------------------------------|
| `--runtime host[:port]` | The address to the management endpoint of a Runtime.               |
| `--output table\|json`  | Select the format the output of the subcommand. Defaults to table. |
| `--wide`                | If set, prints more details in table format for a wider output.    |
| `--help`                | Show help information.                                             |

### Details

The `dolittle runtime` commands interacts with a Runtime you can access from your machine.
You can specify an endpoint using the `--runtime <host[:port]>` option.
If you don't specify an endpoint, the CLI will try to locate a Runtime it can interact with itself.
Currently it looks for Docker containers running a `dolittle/runtime:*` image with the management port (51052) exposed.
If there are more than one available Runtime and you have not specified an endpoint, you'll be presented with an interactive selector to choose one.

### Subcommands
