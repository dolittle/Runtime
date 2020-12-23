---
title: Overview
description: The overview of Dolittle
weight: 1
repository: https://github.com/dolittle/Runtime
---



The Dolittle stack is composed of the SDK, the Runtime and the Event Store.


![anatomy](/images/concepts/anatomy.jpg)


In short, the Dolittle Runtime manages the connections from the SDK's and from other Runtimes to the [Event Store](). It's the backend of our system.

It's called the Runtime as it's what runs and powers the SDK's to do their job.

Events are "facts that have happened" and they are stored in the {{< ref "event-store" >}}.

### Technicalities

The connection between the Runtime and the SDK's is managed through [gRPC]() call's

[Event Store]() is blah blah
