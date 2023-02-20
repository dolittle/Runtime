---
title: Configuration
description: Runtime configuration files reference
weight: 10
---

The [Runtime]({{< ref "docs/concepts/overview" >}}) uses the [ASP.NET Configuration System](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0) for reading configuration related to
setting up the logging and also configuration for the Runtime itself. It reads and overrides configuration in a prioritized manner where configuration from files is overridden by environment variables and command-line arguments.

# Configuration points

## Endpoints
Sets up the ports for the Runtime interface endpoints.

### Private
The GRPC port for communication between Runtime and Clients, usually an application using the SDK, for Runtime functionalities.
* Port
  * Type: Integer
  * Default: 50053
### Public
The GRPC port for communication between Runtimes using the Event Horizon.
* Port
  * Type: Integer
  * Default: 50052
### Management
The GRPC port for communication between Runtimes and Clients for management related functionalities. The ´dolittle´ CLI uses this port for communicating with the Runtime.
* Port
  * Type: Integer
  * Default: 51052
### ManagementWeb
The GRPC-Web port for communication between Runtimes and Clients for management related functionalities. A browser-application using GRPC-Web can communicate with the Runtime using this port.
* Port
  * Type: Integer
  * Default: 51152
### Web
The REST-service endpoint for the Runtime. Only some functionalities are provided through this endpoint.
* Port
  * Type: Integer
  * Default: 8001
### Metrics
The port where we provide metrics using Prometheus. Used for metrics scraping.
* Port
  * Type: Integer
  * Default: 9700


## Eventstore:BackwardsCompatibility:Version
A required configuration denoting whether the event store should use the old formatting used in V6 of the runtime or the newer format. In V6 Runtime the Partition ID and Event Source ID were forced to by GUIDs and also stored as such in the database. In V7 we changed it so that these were strings, not GUIDs, thus the database scheme is slightly different and it needs to be configured explicitly. Only Runtimes that has event store databases with data from V6 of the Runtime should use the V6 option.

### Type: String
### Values
* V6
* V7
## ReverseCalls
Contains a feature-flag for whether or not to use reverse calls implemented with actors.
* UseActors
  * Type: boolean
  * Default: True

## Platform
Defines the platform specific information related to the running microservice.
* CustomerName
  * Type: string
  * Default: '[Not Set]' 
* CustomerID
  * Type: GUID
  * Default: 'ca900ec9-bae8-462e-b262-fa3efc825ca8'
* ApplicationName
  * Type: string
  * Default: '[Not Set]' 
* ApplicationID
  * Type: GUID
  * Default: '4fe9492c-1d19-4e6b-be72-03208789906e'
* MicroserviceName
  * Type: string
  * Default: '[Not Set]' 
* MicroserviceID
  * Type: GUID
  * Default: '4a5d2bc3-543f-459a-ab0b-e8e924093260'
* Environment
  * Type: string
  * Default: 'Development'

## Microservices
The addresses of other Runtimes used when subscribing to an event horizon stream.
This is a dictionary mapping specifying the host:port address for reaching the Public Endpoint on the given Microservice ID (Key of the objet)
* \<Microservice-GUID\>
  * Host
    * Type: string
  * Port
    * Type: Integer

# Tenant-Specific Configuration
Configurations that are specific to a single tenant. All of the configurations for a tenant is behind the configuration key tenants:\<Tenant-Guid\> 
## Resources
### Embeddings [Obsolete]
* Servers
  * Description: The name of the host server. (Usually 'localhost')
  * Type: Array[string]
* Database
  * Description: The name of the mongo database 
  * Type: string
* MaxConnectionPoolSize
  * Description: The max number of concurrent MongoDB connections
  * Type: Integer
  * Default: 1000
### EventStore
* Servers
  * Description: The name of the host server. (Usually 'localhost')
  * Type: Array[string]
* Database
  * Description: The name of the mongo database 
  * Type: string
* MaxConnectionPoolSize
  * Description: The max number of concurrent MongoDB connections
  * Type: Integer
  * Default: 1000
### Projections
* Servers
  * Description: The name of the host server. (Usually 'localhost')
  * Type: Array[string]
* Database
  * Description: The name of the mongo database 
  * Type: string
* MaxConnectionPoolSize
  * Description: The max number of concurrent MongoDB connections
  * Type: Integer
  * Default: 1000

### ReadModels
* Host
  * Description: The full host connection string of the MongoDB connection. (Usually `mongodb://localhost:27017`)
  * Type: string
* Database
  * Description: The name of the mongo database 
  * Type: string
* MaxConnectionPoolSize
  * Description: The max number of concurrent MongoDB connections
  * Type: Integer
  * Default: 1000

## EventHorizon
Defines the event horizons for this tenant (only consents can be configured).
* \<Microservice-GUID\>
  * Consents
    * Description: The list of consents for a specific `Partition` in a public `Stream` from this specific tenant to specific tenant in the `Microservice` from the key of this entry.
    * Type: Array[Object]
      * ConsumerTenant
        * Description: The ID of the `Tenant` in the `Microservice` given above in the key of this entry that gets a consent to subscribe to the configured `Partition` in the configured public `Stream`
        * Type: GUID
      * Stream
        * Description: The ID of the public stream that the event horizon reads events from.
        * Type: GUID
      * Partition
        * Description: The partition ID of the public stream that the event horizon reads events from.
        * Type: GUID
      * Consent
        * Description: The ID of the consent. (Not used for anything yet)
        * Type: GUID

## `runtime.yml`
The [Runtime]({{< ref "docs/concepts/overview" >}}) uses a single YAML configuration file called `runtime.yml` located under the `.dolittle/` folder. This file has a 1:1 correspondence with the Runtime Configuration meaning that all fields under the runtime.yml config file gets prefixed `Dolittle:Runtime:` (represented as an environment variable by `Dolittle__Runtime__`) used in the Asp.Net configuration system.

### Example config
```yaml runtime.yml
eventStore:
    backwardsCompatibility:
        version: V6
platform:
    customerID: 6d8eaf84-969c-4234-b78f-30632a608e5a
    applicationID: e0078604-ae62-378d-46fb-9e245d824c61
    microserviceID: ffb20e4f-9227-574d-31aa-d6e59b34495d
    customerName: TheCustomer
    applicationName: THeApplication
    microserviceName: TheMicroservice
    environment: Dev
microservices:
    d47c6fb7-2339-e286-2912-2b9f163a5aa3:
        host: some.host
        port: 50052
tenants:
    1c707441-95b3-4214-a4d1-4199c58afa23:
        resources:
            embeddings:
            servers:
            - my.host
            database: embeddings
            eventStore:
            servers:
            - my.host
            database: eventstore
            readModels:
            host: mongodb://my.host:27017
            database: readmodels
            projections:
            servers:
            - dev-mongo.application-e0078604-ae62-378d-46fb-9e245d824c61.svc.cluster.local
            database: projections
        eventHorizons:
            d47c6fb7-2339-e286-2912-2b9f163a5aa3:
            consents:
            -   consumerTenant: c5b5847a-68e6-4b31-ad33-8f2beb216d8b
                stream: d9b302bb-5439-4226-a225-3b4a0986f6ed
                partition: 00000000-0000-0000-0000-000000000000
                consent: 4d43e837-0a8e-4b3d-a3eb-5301f5650d91
```
### Default configuration
When using the `dolittle/runtime` images it is provided with a default `runtime.yml` setting up only resources for the "`Development Tenant`" `445f8ea8-1a6f-40d7-b2fc-796dba92dc44`
```yaml
tenants:
  445f8ea8-1a6f-40d7-b2fc-796dba92dc44:
    resources:
      eventStore:
        servers:
          - localhost
        database: event_store
        maxConnectionPoolSize: 1000
      projections:
        servers:
          - localhost
        database: projections
        maxConnectionPoolSize: 1000
      embeddings:
        servers:
          - localhost
        database: embeddings
        maxConnectionPoolSize: 1000
      readModels:
        host: mongodb://localhost:27017
        database: readmodels
        useSSL: false
```

{{< alert title="Note" color="info" >}}
In addition the  `dolittle/runtime` -development tags also sets the `EventStore Compatibility Version` to `V7`
{{< /alert >}}

## `appsettings.json`
All Runtime configurations in theory can also be provided through the Asp.Net `appsettings.json` configuration file by simply having a Dolittle:Runtime object in the root of the configuration:

```json
{
    "Dolittle": {
        "Runtime": {
            "Platform": {
                "ApplicationID": ...
            },
            "Tenants": {
                "<Tenant-GUID>": {
                    "Resources": {
                        ...
                    }
                }
            }
        }
    },
    "Logging": {
        ...
    }
}

```

## Environment variables
All configurations to the Runtime can be configured with environment variables by prefixing the environment variables with `Dolittle__Runtime__`

# Legacy

The Legacy [Runtime]({{< ref "docs/concepts/overview" >}}) pre version V8 uses JSON configuration files. The files are mounted to the `.dolittle/` folder inside the Docker image.

| Configuration file            | Required |
|-------------------------------|----------|
| `platform.json`               | ✔️        |
| `tenants.json`                | ✔️        |
| `resources.json`              | ✔️        |
| `event-horizon-consents.json` | ✔️        |
| `microservices.json`          |          |
| `metrics.json`                |          |
| `endpoints.json`              |          |


{{< alert title="Note" color="info" >}}
These files can be used to override configuration provided by default or by `runtime.yml`. The `tenants.json` is also completely obsolete and should not be used.
{{< /alert >}}
{{< alert title="Note" color="warning" >}}
The legacy file provider for `event-horizon-consents.json` is buggy. So if a Runtime needs to have event horizon consents configured then the configuration needs to be provided through the `runtime.yml` configuration (or environment variables).
{{< /alert >}}

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
