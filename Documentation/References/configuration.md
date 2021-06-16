---
title: Configuration
description: Runtime configuration files reference
weight: 10
---

The [Runtime]({{< ref "docs/concepts/overview" >}}) uses JSON configuration files. The files are mounted to the `.dolittle/` folder inside the Docker image.

| Configuration file            | Required |
|-------------------------------|----------|
| `tenants.json`                | ✔️        |
| `resources.json`              | ✔️        |
| `event-horizon-consents.json` | ✔️        |
| `microservices.json`          |  ️        |
| `metrics.json`                |          |
| `endpoints.json`              |          |

## `tenants.json`
**Required.** Defines each [Tenant]({{< ref "docs/concepts/tenants" >}}) in the Runtime.
```json
{
    <tenant-id>: {}
}
```

## `resources.json`
**Required.** Configurations for the [Event Store]({{< ref "docs/concepts/event_store" >}}) per [Tenant]({{< ref "docs/concepts/tenants" >}}).
```json
{
    <tenant-id>: {
        "eventStore": {
            "servers": [
                <MongoDB connection URI>
            ],
            "database": <MongoDB database name>,
            // defaults to 1000. MongoDB max connection amount
            "maxConnectionPoolSize": 1000
        }
    }
}
```

## `event-horizon-consents.json`
**Required.** Defines the [Consents]({{< ref "docs/concepts/event_horizon#consent" >}}) a [Producer]({{< ref "docs/concepts/event_horizon#producer" >}}) tenant gives to [Consumers]({{< ref "docs/concepts/event_horizon#consumer" >}}) so that they can receive events over the [Event Horizon]({{< ref "docs/concepts/event_horizon" >}}).
```json
{
    // The producer tenant that gives the consent
    <tenant-id>: [
        {
            // the consumers microservice and tenant to give consent to
            "microservice": <microservice-id>,
            "tenant": <tenant-id>,
            // the producers public stream and partition to give consent to
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
Defines where the [Producer]({{< ref "docs/concepts/event_horizon#producer" >}}) microservices are so that the [Consumer]({{< ref "docs/concepts/event_horizon#consumer" >}}) can [Subscribe]({{< ref "docs/concepts/event_horizon#subscription" >}}) to them.
```json
{
    // the id of the producer microservice
    <microservice-id>: {
        // producer microservices Runtime host and public port
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
