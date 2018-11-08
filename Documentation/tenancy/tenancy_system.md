---
title: Tenancy System
description: About the tenancy system
keywords: tenancy, system 
author: woksin
---

# Tenancy
The tenant is a vital piece of information which describes to the Dolittle Runtime which context the system is in. The runtime system is fundamentally built to be multi-tenant, it relies upon the execution context of the system to determine which tenant the runtime is configured for.

## Tenant Resolving System
We're providing with a system that sets the correct tenant on the runtime's execution context whenever an action is performed. The tenant resolving system provides with a mechanism to configure an application to use a configured tenant resolving strategy.  

### Configuration
A single json file called tenant-map.json is used to configure the Tenant Resolving System. The configuration file is located inside the .dolittle folder and has the following structure:

```json
{
    "strategy": "<Strategy Name>",
    "fallbackToDeveloperTenant": true,  
 }
```
This is the standard skeleton for every tenant-map.json configuration, the content of the configuration is dependent on the actual strategy that's in use.

For example, in our AspNetCore implementation we have defined a strategy for resolving the tenant by hostname (You can find this strategy [here](https://github.com/dolittle-interaction/AspNetCore/tree/master/Source/Tenancy/Strategies/Hostname))

For this particular strategy the actual configuration of the tenant-map.json configuration could looks like this:

```json
{
    "strategy": "hostname",
    "fallbackToDeveloperTenant": false,
    "configuration": {
        "segments": "first"
    },
    "map": {
       "TenantA": "720ab91f-23b7-44f2-bde2-0638214971ec",
       "TenantB": "7269089b-8ebd-4516-a06e-7c95fc6b816b"
    }
 }
```
{{% notice info %}}
The definition of this particular configuration can be found [here](https://github.com/dolittle-interaction/AspNetCore/blob/master/Source/Tenancy/Strategies/Hostname/HostnameStrategyResource.cs)
{{% /notice %}}