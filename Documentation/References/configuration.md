---
title: Configuration
description: Runtime configuration files reference
weight: 10
---

The [Runtime]({{< ref "docs/concepts/overview" >}}) uses JSON configuration files. The files are mounted to the `.dolittle/` folder inside the Docker image.

| Configuration file            | Required |
|-------------------------------|----------|
| `platform.json`               | ✔️        |
| `tenants.json`                | ✔️        |
| `resources.json`              | ✔️        |
| `event-horizon-consents.json` | ✔️        |
| `microservices.json`          |          |
| `metrics.json`                |          |
| `endpoints.json`              |          |

## `platform.json`
**Required.** Configures the Microservice environment for the Runtime.
```json
{
    "applicationName": "<application-name>",
    "applicationID": "<application-id>",
    "microserviceName": "<microservice-name>",
    "microserviceID": "<microservice-id>",
    "customerName": "<customer-name>",
    "customerID": "<customer-id>",
    "environment": "<environment-name>"
}
```

## `tenants.json`
**Required.** Defines each [Tenant]({{< ref "docs/concepts/tenants" >}}) in the Runtime.
```json
{
    <tenant-id>: {}
}
```

## `resources.json`
**Required.** Configurations for the resources available per [Tenant]({{< ref "docs/concepts/tenants" >}}):
- `eventStore`: MongoDB configuration for the [Event Store]({{< ref "docs/concepts/event_store" >}})
- `projections`: MongoDB configuration for the storage of [Projections]({{< ref "docs/concepts/projections" >}})
- `embeddings`: MongoDB configuration for the storage of [Embeddings]({{< ref "docs/concepts/embeddings" >}})
- `readModels`: MongoDB configuration for a database that can be used for any storage and accessed through the SDKs directly. This database should only be used to store data that can be rebuilt from replaying events.

The `database` name must be unique for all resources and tenants, reusing the same name will cause undefined behaviour in the Runtime and potential dataloss.
```json
{
    <tenant-id>: {
        "eventStore": {
            "servers": [
                <MongoDB connection URI>
            ],
            "database": <MongoDB database name>,
            "maxConnectionPoolSize": 1000
        },
        "projections": {
            "servers": [
                <MongoDB connection URI>
            ],
            "database": <MongoDB database name>,
            "maxConnectionPoolSize": 1000
        },
        "embeddings": {
            "servers": [
                <MongoDB connection URI>
            ],
            "database": <MongoDB database name>,
            "maxConnectionPoolSize": 1000
        },
        "readModels": {
            "host": <MongoDB connection string>,
            "database": <MongoDB database name>,
            "useSSL": false
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
