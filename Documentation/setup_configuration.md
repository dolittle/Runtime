---
title: Setup configuration
description: Setup runtime with correct configuration files
keywords: setup, configuration, json, resources, tenancy
author: joel
---

To run a runtime with a had you have to provide some JSON configuration files to both the runtime and the head.
If you are using our Dolittle Platform some of these configurations will be provided for you, others you have to manually set yourself.

| Configuration               | Head | Runtime | Required | Provided by Dolittle Platform
|-----------------------------|------|---------|----------|----------|
| event-horizons.json         | ✔️    |         |          |          |
| clients.json                | ✔️    |         |          | ✔️        |
| tenants.json                | ✔️    | ✔️       | ✔️        | ✔️        |
| resources.json              | ✔️    | ✔️       | ✔️        | ✔️        |
| microsevices.json           |      | ✔️       | ✔️        | ✔️        |
| endpoints.json              |      | ✔️       |          | ✔️        |
| metrics.json                |      | ✔️       |          | ✔️        |
| event-horizon-consents.json |      | ✔️       |          | ✔️        |


## Runtime

### resources.json
Configurations for the read models and event store.
```json
{
    <tenant-id>: {
        "readModels": {
            "host": <MongoDB host>,
            "database": <MongoDB database name>,
            "useSSL": false
        },
        "eventStore": {
            "servers": [
                "localhost"
            ],
            "database": <MongoDB database name>
        }
    }
}

```

### tenants.json
The tenants in the system.
```json
{
    <tenant-id>: {}
}
```

### metrics.json
The port to expose the metrics server on.
```json
{
    "Port": <port>
}
```


### endpoints.json
Ports for the public and private which to the head can connect.
```json
{
    "public": {
        "port": <port>
    },
    "private": {
        "port": <port>
    }
}
```

### event-horizon-consents.json
Each tenant can give consents to a set of entities given by their id's.
```json
{
    <tenant-id>: [
        {
            "microservice": <microservice-id>,
            "tenant": <tenant-id>,
            "stream": <stream-id>,
            "partition": <parition-id>,
            "consent": <consent-id>
        },
        { ... }
    ]
}
```

### microservices.json
Defines the microservices configuration
```json
{
    <microservice-id>: {
        "host": <host>,
        "port": <port>
    }
}
```
