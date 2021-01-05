---
title: Configuration
description: Runtime configuration files reference
weight: 10
---

The [Runtime]({{< ref "concepts/overview" >}}) uses JSON configuration files to define it's tenants and [Event Horizon]({{< ref "concepts/event_horizon" >}}) consents. They are mounted inside the Docker image inside the `.dolittle/` folder.

| Configuration file            | Required |
|-------------------------------|----------|
| `tenants.json`                | ✔️        |
| `resources.json`              | ✔️        |
| `event-horizon-consents.json` | ✔️        |
| `microservices.json`          |  ️        |
| `metrics.json`                |          |
| `endpoints.json`              |          |

## `tenants.json`
**Required.** Defines each tenant in the Runtime. 
```json
{
    <tenant-id>: {}
}
```

## `resources.json`
**Required.** Configurations for the event store per tenant. 
```json
{
    <tenant-id>: {
        "eventStore": {
            "servers": [
                <MongoDB connection URI>
            ],
            "database": <MongoDB database name>,
            // optional. MongoDB max connection amount
            "maxConnectionPoolSize": 1000
        }
    }
}
```

## `event-horizon-consents.json`
**Required.** Defines the consents a producer tenant gives to consuming `microservices` so that they can receive events over the [Event Horizon]({{< ref "concepts/event_horizon" >}}).
```json
{
    // The producer tenant that gives the consent
    <tenant-id>: [
        {
            // the consumer microservice and tenant to give consent to
            "microservice": <microservice-id>,
            "tenant": <tenant-id>,
            // the public stream to receive events to
            "stream": <stream-id>,
            "partition": <partition-id>,
            // an identifier for this consent 
            "consent": <consent-id>
        }
    ]
}
```
{{< alert title="Note" color="info" >}}
If there are no subscriptions, the file should only contain an empty JSON object `{}`.
{{< /alert >}}

## `microservices.json`
Defines where the external microservices are so that the [Event Horizon]({{< ref "concepts/event_horizon" >}}) can find them.
```json
{
    // the id of the external microservice
    <microservice-id>: {
        "host": <host>,
        "port": <port>
    }
}
```

## `endpoints.json`
Defines the private and public ports for the Runtime.
```json
{
    "public": {
        // default 50052
        "port": <port>
    },
    "private": {
        // default 50053
        "port": <port>
    }
}
```

## `metrics.json`
The port to expose the Prometheus Runtimes metrics server on.
```json
{
    // default 9700
    "Port": <port>
}
```
