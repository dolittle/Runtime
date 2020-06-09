---
title: Resource System
description: About the resource system
keywords: resources, system 
author: woksin
aliases:
    - /runtime/runtime/resources/resources
---

# Resource
In this context when we're talking about resources, we're talking about the data resources of each *Bounded Context*. This included the [**Event Store**]({{< relref event_store >}}) and the Read models database.

## Resource System
We're providing a system that automatically hooks up the previously mentioned data stores up with the correct configurations on our implementations of the stores based on a small set of configurations in the bounded-context.json file and a resources.json file found inside the .dolittle folder. With the help of the Dolittle Execution Context management system, the Resource System plays an important role in enabling the Runtime to behave in a multi tenant context.

### Resources configuration
Based on two configurations we can setup the resource system:


##### bounded-context.json: 

We need to tell the runtime which implementations of the different stores to use for each environment. The implementation name must be defined in the implementations we create for the different stores.

Dolittle currently only offers a MongoDB implementation of the Event store and Read models database. The implementation name string for the MongoDB implementation is **"MongoDB"**
```json
{
    "application": "<Some GUID>",
    "boundedContext": "<Some GUID>",
    "boundedContextName": "<Bounded Context Name>",
    "resources": {
        "readModels": {
            "production": "<Implementation name>",
            "development": "<Implementation name>"
        },
        "eventStore": {
            "production": "<Implementation name>",
            "development": "<Implementation name>"
        }
    }
}
```
##### resources.json

This is where we provide the actual configuration of the Read models and the Event store connections for the given tenants.

The store implementations (defined in the configuration above) will have their own configurations for setting up database connections (shown below is the configuration for the MongoDB implementations for Event store and Read models)

{{% notice info %}}
When doing development on a Dolittle-based application, the easiest thing to do is to set up the configuration for a well-known development tenant that we provide: "445f8ea8-1a6f-40d7-b2fc-796dba92dc44"

When the application is going into production, the appropriate production configurations should be mounted up manually. 
{{% /notice %}}

```json
{
    "<Some Tenant Id>": {
        "readModels": {
            "connectionString": "<Some Connection String>",
            "host": "<Host>",
            "database": "<Database name>",
            "useSSL": false
        },
        "eventStore": {
            "connectionString": "<Some Connection String>",
            "host": "<Host>",
            "database": "<Database name>",
            "useSSL": false
        }
    }
}
```

{{% notice warning %}}
It's important that when doing local development that you also have a 'tenants.json' file in the .dolittle folder that is setup with the tenant that you provide resource configurations for. For example if you have this resources.json configuration:

```json
{
    "445f8ea8-1a6f-40d7-b2fc-796dba92dc44": {
        "readModels": {
            "host": "mongodb://localhost:27017",
            "database": "read_model_database",
            "useSSL": false
        },
        "eventStore": {
            "host": "mongodb://localhost:27017",
            "database": "event_store",
            "useSSL": false
        }
    }
}
```
you need to provide the following tenants.json configuration:
```json
{
    "tenants": {
        "445f8ea8-1a6f-40d7-b2fc-796dba92dc44": {}
    }
}
```
{{% /notice %}}
