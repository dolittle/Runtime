---
title: Setup configuration
description: Setup runtime with correct configuration files
keywords: setup, configuration, json, resources, tenancy
author: joel
---

To run a runtime with a had you have to provide some configuration files to both the runtime and SDK. These files are in JSON format.

## Runtime

### tenants.json
The tenants in the system. (mounted from head?)
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


## Head

### tenants.json
The tenants in the head.
```json
{
    <tenant-id>: {}
}
```

### event-horizons.json
Defines a event horizon subscription for a tenant.
```json
{
    <tenant-id>: [
        {
            "scope": <scope-id>,
            "microservice": <microservice-id>,
            "tenant": <tenant-id>,
            "stream": <stream-id>,
            "partition": <parition-id>,
        }
    ]
}
```

### clients.json
Ports which to connect to in the runtime.
```json
{
    "public": {
        "host": <host>,
        "port": <port>
    },
    "private": {
        "host": <host>,
        "port": <port>
    }
}
```
