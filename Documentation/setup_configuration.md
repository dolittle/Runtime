---
title: Setup configuration
description: Setup runtime with correct configuration files
keywords: setup, configuration, json, resources, tenancy
author: joel
---

To run a runtime with the SDK you have to provide some configuration files to both the runtime and SDK. These files are in JSON format.

## Runtime

### tenants.json
The tenants in the system. (mounted from head?)
```json
{
    <tenant-id>: {}
}
```

### metrics.json
This port is opened for requesting the runtime for metrics.
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
Ports for the public and private 
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
Each tenant has a set of consents 
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
Defines the EventHorizons the head uses.
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

{{% alert info %}}
We have a [Github repository](https://github.com/dolittle-tools/) where our public tools will be kept. We welcome anyone to have a look at the source code and we'd be more than happy for any pull request, issues and feature request!
{{% /alert %}}

## Tools
* [**DotNET Build Tool**](https://dolittle.io/dotnet-sdk/tooling/)
* [**CLI Tool**](https://dolittle.io/cli/) 

## Languages we currently support
{{% notice note %}}
Note that these are the languages that we currently are supporting on our platform and will eventually have tools for. The tools will be extended to support more languages when new SDKs for other programming languages are developed.
{{% /notice %}}

* Core (Backend)
    * C#

* Frontend
    * Javascript 201x