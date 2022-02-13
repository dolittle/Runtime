---
title: Replay
description: Initiates a replay of a Projection currently registered by Clients to the Runtime
weight: 30
repository: https://github.com/dolittle/Runtime
---

Initiates a replay of all events to rebuild Projection read models. This command drops all persisted read models, and potential copies in MongoDB, and restarts the Projection to build new ones.

```shell
dolittle runtime projection replay <identifier> <scope> [options]
```

### Arguments

| Argument       | Description                                                                                                    |
|----------------|----------------------------------------------------------------------------------------------------------------|
| `<identifier>` | The identifier or alias of the Projection to replay                                                            |
| `<scope>`      | The scope of the Projection to replay. Only required when the identifier or alias matches multiple projections |

### Options

| Option                  | Description                                                                                              |
|-------------------------|----------------------------------------------------------------------------------------------------------|
| `--tenant <id>`         | Only replay the Projection for the specified Tenant. If not provided, replays for all Tenants            |
| `--runtime host[:port]` | The address to the management endpoint of a Runtime. See [details]({{< ref "../../Runtime#details" >}}). |
| `--help`                | Show help information.                                                                                   |
