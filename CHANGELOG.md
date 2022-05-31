# [8.1.1] - 2022-5-31 [PR: #695](https://github.com/dolittle/Runtime/pull/695)
## Summary

The filter validator for public filters would take the whole event log into memory and compare filtered streams in memory. This could cause OutOfMemoryException if the event log is big enough

### Fixed

- Fixes `OutOfMemoryException` caused by filter validator for public filters.


# [8.1.0] - 2022-5-30 [PR: #682](https://github.com/dolittle/Runtime/pull/682)
## Summary

Extending upon the event store actors to enable us to write more efficient implementations for event handlers. This is an opt-in feature which requires the `DOLITTLE__RUNTIME__PROCESSING__EVENTHANDLERS__FAST` flag to be set to `TRUE`

### Added

- dolittle:runtime:processing:eventhandlers:fast configuration for enabling fast event handler implementation using actors to efficiently write events to streams
- Integration tests for event handlers


# [8.0.2] - 2022-5-27 [PR: #693](https://github.com/dolittle/Runtime/pull/693)
## Summary

A bug with the structure of the event horizon consent configuration restricted having multiple consents configured for the same consumer microservice and tenant.

### Added

- Describe the added features

### Changed

- When starting up the Runtime it will crash if the event store compatability version configuration is not set
- Sets log level for proto actor framework to Warning

### Fixed

- An issue related to the event horzion consents configuration restricting having multiple consents set up for the same consumer microservice and tenant
- Fixed an issue with the Dockerfiles environment variables for setting the console logger


# [8.0.1] - 2022-4-20 [PR: #659](https://github.com/dolittle/Runtime/pull/659)
## Summary

Start integrating the Actor Model into the Runtime code with first introducing [Proto.Actor](https://proto.actor) into the Event Store. We're taking advantage of the Actor Model to implement batching logic on the write side of the Event Store (committing events) to massively increase the throughput for committing events. This is also built in a way that enables us to in the future be able to scale horizontally through having multiple Runtimes running together in clusters.

In addition we now do benchmarking on key parts of the system and publishing the results on each GitHub release so that we can keep an eye on the current performance. 

### Changed

- When committing events the commits will be written and responded in batches which in turn increases the throughput of commits by several times.


# [8.0.0] - 2022-3-25 [PR: #654](https://github.com/dolittle/Runtime/pull/654)
## Summary

The breaking change from v6 to v7 was that we changed `EventSource` and `Partition` from Guids to Strings - both in the Event Store schema and the Contracts. This ment that upgrading from v6 required both a full upgrade of all microservices that is connected through Event Horizon, and a MongoDB migration tool (that we never completed). This was sub-optimal.

With this release, we introduce another breaking change, to un-break these changes. This means that a v8 Runtime is fully compatible with a v6 Runtime - all you need to do is upgrade the SDK. The v8 Runtime is also compatible with the Event Store from a v7 Runtime, **but not the Event Horizon**. We consider this a non-issue since it is not used by anyone to our knowledge.

To use Runtime v8, you need to:
- Update your SDK to `17.0.0` for .NET, or `24.0.0` for JavaScript.
- Start the Runtime with an environmental variable called `DOLITTLE__RUNTIME__EVENTSTORE__BACKWARDSCOMPATIBILITY__VERSION` set to either `V6` or `V7`.
This configures how the v8 Runtime will write to the Event Store so that you can roll back to your previous version in case anything goes wrong. 

### Added

- Reading configuration from a single file called `runtime.json` in the current working directory, environmental variables, and command line arguments. The configuration specified in the `runtime.json` file will be overwritten by the previous configuration files (`resources.json`, `tenants.json`, ...) so that it is still backwards compatible with older setups.

### Changed

- The Event Horizon contracts are compatible with v6 and v8 of the Runtime.
- The EventStore schema is configurable to be compatible with v6 or v7 of the Runtime. This __must__ be configured through the `DOLITTLE__RUNTIME__EVENTSTORE__BACKWARDSCOMPATIBILITY__VERSION` environmental variable, otherwise the Runtime will refuse to commit or process any events.


# [7.8.1] - 2022-3-25 [PR: #636](https://github.com/dolittle/Runtime/pull/636)
## Summary

We have refactored a lot of the internals of the Runtime - to remove code that we no longer need, and to rely more on well tested frameworks over own code. Notable examples:
- The booting system is gone, we only discover DI container bindings and use the .NET hosting model for the rest
- The resources system is gone, and replaced with "normal" configuration. We don't need multiple implementations of the same resource types.
- The configuration system was replaced with the Microsoft Configuration system, which should make it easier to load configuration from multiple sources - and in the future support hot-reloading.
- The `ExecutionContextManager` that used `AsyncLocal` storage is gone, and we pass along the `ExecutionContext` explicitly or resolve dependencies from tenant-specific containers.
- Removed the custom DepenencyInjection setup, and replaced it with AutoFac and a tenant-specific child container structure that allows us to ensure we don't mix dependencies for different tenants.
- Upgraded from the native Grpc libraries to the C# Grpc libraries that uses AspNetCore (Kestrel) for hosting.

### Added

- A HealthCheck endpoint is exposed on the "web" endpoint (defaults to 8001) on `/healthz`. This endpoint returns 200 if the configuration files are correctly formatted, the MongoDB databases are reachable and the gRPC services running. Otherwise it returns an error and a JSON structure describing the issues.
- The "web" endpoint port is configurable in the `endpoints.json` file.
- More metrics for `ReverseCallDispatcher`

### Changed

- Change from the `Grpc.Core` native packages, to the `Grpc.AspNetCore` C# packages
- The Docker Images that are pushed to Docker Hub are now multi-architecture, supporting `amd64` and `arm64`.

### Fixed
- The `RetryTime` that was passed along from the SDKs when a Projection failed was not used, meaning that failing projections were never retried.
-  Filters that failed to write the filtered event to the stream, threw an error propagated to the SDK instead of handling it as a filter failure and retrying later.
- ReverseCalls where the `Connect` arguments were invalid, caused the SDKs to receive a generic gRPC error instead of the failure that occurred in the Runtime.
 
### Deprecated 
- The `-arm64` images on Docker Hub will no longer be released. The "normal" images should be used in place.


# [7.8.0] - 2022-2-14 [PR: #627](https://github.com/dolittle/Runtime/pull/627)
## Summary

A new management service for Projections, and accompanying CLI commands to get the status and details for registered Projections, and a command to manually force a replay of a Projection. A replay of a projection involves removing all the persisted read models (and potential copies in MongoDB), and restarting the processing from the first event in the Event Store. This can be used to force a replay for changes in Projections that are not automatically detected (e.g. code changes), or to fix copies that have been changed by mistake.

An unrelated, but also useful change is the gRPC Server Reflection service that has been added to all the endpoints. This makes it easier to interact with the Runtime while testing out things, without having to write a client in code.

### Added

- Projections can now register with an Alias
- Implemented the Projections management service to List, Get and Replay registered Projections
- Exposed the gRPC Server Reflection service on all endpoints. This allows for easier debugging and testing of gRPC endpoints using tools like Postman, grpc_cli, or gRPCurl
- CLI commands "projections list", "projections get <id or alias> <scope>", "projections replay <id or alias> <scope>", to interact with the Projections management service.

### Fixed

- A bug in the Handshake service version compatibility checker rejected connections from a Head that was using a version of Contracts with the same major but a lesser minor (e.g. Runtime using v6.8.0 and Head using v6.7.0). This should be allowed, and is now fixed.
- Implemented the GetOne endpoint on the Event Handler management service, and changed the CLI to use this method when getting a single Event Handler. This reduces a bit of work and traffic.


# [7.7.1] - 2022-3-28
## Summary

Manually released new Docker Images that sets the `ASPNETCORE_URLS` environment variable to serve the Web endpoint on port `8001` by default, instead of `80`. This caused issues when running the Runtime in our platform.

### Fixed

- Sets the `ASPNETCORE_URLS` environment variable to to `"http://+:8001"` so that it binds the Web endpoint to port `8001` instead of `80`


# [7.7.0] - 2022-2-11 [PR: #625](https://github.com/dolittle/Runtime/pull/625)
## Summary

Implements version 6.7.0 of Contracts adding two new event key selectors to projections, `StaticKey` and `KeyFromEventOccurred`

### Added

- `StaticKey` event key selector attribute for projection On-methods that sets a constant, static, key as the key of the read model
- `KeyFromEventOccurred` event key selector for projection On-methods that uses the event occurred metadata as the key for the projection read models formatted as the string given to the attribute. We currently support these formats:
    - yyyy-MM-dd
    - yyyy-MM
    - yyyy
    - HH:mm:ss
    - hh:mm:ss
    - HH:mm
    - hh:mm
    - HH
    - hh
    - yyyy-MM-dd HH:mm:ss
    - And the above in different orderings
- Projection definition in the Projection Store for MongoDB has two new fields, `StaticKey` and `OccurredFormat`


# [7.6.1] - 2022-3-28
## Summary

Manually released new Docker Images that sets the `ASPNETCORE_URLS` environment variable to serve the Web endpoint on port `8001` by default, instead of `80`. This caused issues when running the Runtime in our platform.

### Fixed

- Sets the `ASPNETCORE_URLS` environment variable to to `"http://+:8001"` so that it binds the Web endpoint to port `8001` instead of `80`


# [7.6.0] - 2022-2-9 [PR: #614](https://github.com/dolittle/Runtime/pull/614)
## Summary

Introduces secondary storage mechanisms for Projection read models, this enables using existing databases as query engines for read models. The Projections that should be copied, are stored in Collections in the MongoDB resource for the Microservice, so that they can be accessed through the MongoDB resource in the SDKs. These collections will be created and dropped as per the copy specification in the Projection registration request. These collections should only be read from the client, and not modified - as this can cause strange behaviour.

### Added

- Implemented MongoDB copy storage for Projection read models, as introduced in https://github.com/dolittle/Contracts/pull/85

### Changed

- The Dolittle CLI now includes container names in the Runtime selection list when multiple Runtimes are detected.


# [7.5.1] - 2022-3-28
## Summary

Manually released new Docker Images that sets the `ASPNETCORE_URLS` environment variable to serve the Web endpoint on port `8001` by default, instead of `80`. This caused issues when running the Runtime in our platform.

### Fixed

- Sets the `ASPNETCORE_URLS` environment variable to to `"http://+:8001"` so that it binds the Web endpoint to port `8001` instead of `80`


# [7.5.0] - 2022-1-24 [PR: #609](https://github.com/dolittle/Runtime/pull/609)
## Summary

Implements the new method introduced in the projection store that retrieves all projection states in batches to allow large and a high number of projection states to be retrieved by a client. The implementation also streams states directly from the underlying MongoDB storage to avoid having to read all into memory before passing along to a client. This provides a fix for the issues of fetching a large number of projections crashing the SDKs.

### Added

- A new `GetAllInBatches` method on the `ProjectionStore` service that streams the responses back to the client in dynamically sized batches of approximately 2MB. Singular projection states that are larger than 2MB are sent in their own batch, as the client might still accept larger gRPC messages.


# [7.4.1] - 2022-3-28
## Summary

Manually released new Docker Images that sets the `ASPNETCORE_URLS` environment variable to serve the Web endpoint on port `8001` by default, instead of `80`. This caused issues when running the Runtime in our platform.

### Fixed

- Sets the `ASPNETCORE_URLS` environment variable to to `"http://+:8001"` so that it binds the Web endpoint to port `8001` instead of `80`


# [7.4.0] - 2022-1-20 [PR: #597](https://github.com/dolittle/Runtime/pull/597)
## Summary

Updated to .net6 and C# 10 - and in the process updated all the code to use new language features (e.g. file scoped namespace and code generated logger messages), and fixed other build warnings in the process. Implemented Handshake service, and added some new and renamed some old concepts to represent the Runtime execution context. Introduced a new `platform.json` config file to be provided by the platform. Added VersionInfo to the Runtime to be baked in during build. Lastly fixed a bug in the Embedding processor that caused the wrong execution context to be passed to the Clients when processing embedding requests, and improved the error that is returned when attempting to register an EventHandler more than once. 

### Added

- Handshake service that checks if the connecting client uses a compatible version of the Contracts, and returns the Runtime execution context configured by `platform.json`
- CLI build for macOS on M1 chips now work, so that binary is added to the release.

### Fixed

- A bug in the Embedding processor that caused the wrong execution context to be passed to Clients when processing embedding requests.
- Improved the failure returned when attempting to register an Event Handler more than once.


# [7.3.0] - 2021-11-18 [PR: #589](https://github.com/dolittle/Runtime/pull/589)
## Summary

Implements Contracts 6.3.0, which adds support for getting the configured Tenants, and connection string for a MongoDB resource per tenant. Note: this MongoDB connection string is configured through the "readModels" key in `resources.json` to be backwards compatible with the platform configuration.

### Added

- TenantsService: Getting all tenants through gRPC
- ResourcesService: Getting a specific resource through gRPC. Currently only the MongoDB is supported through the readModels resource configuration


# [7.2.0] - 2021-11-5 [PR: #582](https://github.com/dolittle/Runtime/pull/582)
## Summary

Registration of Aggregate Roots and Event Types can now be done in order to provide an alias to id mapping

### Added

- Runtime Services:
    - AggregateRoots.RegisterAlias: Register an alias mapping for an Aggregate Root
    - EventTypes.Register: Register an EventType with an optional alias
- Runtime Management Services:
    - AggregateRoots.GetAll: Gets all registered Aggregate Roots
    - AggregateRoots.GetOne: Gets information about a specific Aggregate Root
    - AggregateRoots.GetEvents: Gets committed events for an Aggregate Root Instance
    - EventTypes.GetAll: Gets all registered Event Types
- Documentation for new CLI commands
- CLI
    - dolittle runtime aggregates list
    - dolittle runtime aggregates get
    - dolittle runtime aggregates events
    - dolittle runtime eventtypes list


# [7.1.1] - 2021-11-3 [PR: #586](https://github.com/dolittle/Runtime/pull/586)
## Summary

Fixes the behaviour of default configuration for gRPC Endpoints. The previous implementation used default values for all endpoints together (the whole contents of the `endpoints.json` file). Meaning that if you partially specified this configuration for some endpoints, it would not use default values for the rest. This fixes that by using the default values per endpoint visibility if not provided in the config Gile.

### Changed

- Use default values for each `EndpointConfiguration` per `EndpointVisibility`, instead of defaulting for the whole `EndpointsConfiguration` only when nothing was provided.


# [7.1.0] - 2021-10-21 [PR: #562](https://github.com/dolittle/Runtime/pull/562)
## Summary

### Added

- The Dolittle CLI tool with commands for getting information about and reprocessing events for running Event Handlers.
- The option to register Event Handlers with a named alias that is picked up and used by the Dolittle CLI for easier tracking of Event Handlers
- [Dolittle CLI Documentation](https://dolittle.io/docs/reference/cli/)

### Changed

- Updated Contracts versions
- Updated Grpc and Protobuf versions
- [Documentation around replaying Event Handlers](https://dolittle.io/docs/concepts/event_handlers_and_filters/#replaying-events)


# [7.0.0] - 2021-10-13 [PR: #556](https://github.com/dolittle/Runtime/pull/556)
## Summary

Implementing the changes introduced by https://github.com/dolittle/Contracts/pull/53. Allowing EventSourceID and PartitionID to be strings, to more easily integrate with events from existing systems.

This is considered a breaking change because it only works with SDKs using the v6 Contracts.

### Added

- EventSourceID is now a string instead of a Guid.
- PartitionID is now also a string instead of a Guid.

### Fixed

- Aligned names of event type fields throughout messages from Contracts v6.0.0

### Removed

- `EventSourceId.NotSet` is removed, as the value should always be set to something. We had some hacks internally where we used this value since we knew it would be thrown away, but now those values are set to something more sensible just to be sure.

# [6.2.0] - 2022-3-15
## Summary

Back porting of readiness health checks to v6 Runtime

### Added

- Back ported Health Checks for validating resources configuration, MongoDB database connections and grpc endpoints connections

# [6.1.0] - 2021-6-29 [PR: #550](https://github.com/dolittle/Runtime/pull/550)
## Summary

Adds Embeddings, a special type of event handler used for comparing read models states and generating events to reflect that changes done to the state. The embeddings are saved into their own database, much like Projections.

### Added

- Grpc services for registering, updating and deleting embeddings
- Grpc services for retrieving embeddings
- New resource definition for embeddings in `resources.json`. This defines the MongoDB database used by the embeddings, the format is the same as for the other databases.
```json
"embeddings": {
    "servers": [
        "localhost"
    ],
    "database": "embeddings",
    "maxConnectionPoolSize": 1000
}
```

### Changed

- Changed some loggs to `Trace`, as they were just clogging the `Debug` logging too much.


# [6.0.1] - 2021-6-16 [PR: #548](https://github.com/dolittle/Runtime/pull/548)
## Summary

Fix some of the configuration files under Server/.dolittle to their original intended 

### Changed

- Changed back configuration files


# [6.0.0] - 2021-6-16 [PR: #532](https://github.com/dolittle/Runtime/pull/532)
## Summary

Changes the behavior of the pinging system to be more reliable and to also start immediately when receiving a connection. This is to deal with a bug that was causing connections between the SDK and the Runtime to be dropped. This is a **breaking behavioral change**, as the pinging behavior now expects the client to be immediately ready to receive pings and write pongs after creating the connection. Older versions of the SDKs wont function correctly with this release of the Runtime. For this we've added a [compatibility table](https://dolittle.io/docs/reference/runtime/compatibility).

We also added a Prometheus metric system into the Runtime, which you can access from `localhost:9700/metrics` to see the full list of all the metrics reported.

Also reworks the reverse call services to be more reliable and fixes an Event Horizon bug logging bug.

### Added

- A Prometheus metrics service exposed on port `9700` and path `/metrics`. It mainly collects metrics related to Event Horizon currently.

### Changed

- The Runtime now starts to ping the client immediately when receiving a connection instead of waiting for the connection to be accepted first. The client needs to be ready to start receiving pings and writing pongs immediately after initiating the connection.
- Reworked many aspects of the reverse call clients and dispatchers to be more readable and reliable.

### Fixed

- Make pinging more reliable by using a single high priority thread to keep track of the scheduled pings. The old implementation relied on the `ThreadPool`, which was swamped during the startup and other periods of high activity in the Runtime, causing the pings to be delayed and eventually timing out.
- Fixes a bug in the event horizon subscription that resulted in it not writing a proper log message of what the cause of a failure was if one of the event horizon processing tasks failed with an exception. ([#544](https://github.com/dolittle/Runtime/pull/544))


# [5.6.0] - 2021-4-29 [PR: #513](https://github.com/dolittle/Runtime/pull/513)
## Summary

Adds a new Filter Validation implementation for the kind of filter (event type with eventsource partition) that validates the filter by checking just the types already committed in the event log rather than iterating through all the events.

For most registrations, the Event Handlers have not changed, which is detected by the validator and skips any further analysis. If the events in the filter has changed, the new implementation checks the types of events committed in the event log to determine if the stream would change given the new definition. Initial tests indicate that this process is a lot faster than re-filtering the streams.

### Added

- A new specialised filter validation implementation used for Event Handlers to speed up the registration process that happens during boot. This drastically reduces the CPU utilisation and duration of the initial registration process for Event Handlers.


# [5.5.0] - 2021-4-9 [PR: #502](https://github.com/dolittle/Runtime/pull/502)
## Summary

Adds Projections, that are a special type of event handler dealing with read models. Each Projection deals with one read model and mutates it's properties. The read models are saved into the Runtime's projection store. Any changes to the Projection causes it to be fully replayed.

### Added

- Grpc service for getting projections
- REST (WebAPI) service for getting projections
- Grpc service for registering and processing projections
- A MongoDB store for projections - stores the definitions in `projection-definitions` and the projections in `projection-projectionid` collections. It's defined in the `resources.json`:
```json
"projections": {
    "servers": [
        "localhost"
    ],
    "database": "projections",
    "maxConnectionPoolSize": 1000
}
```

### Changed

- Adhere to latest protobuf contracts


# [5.4.2] - 2021-2-4 [PR: #500](https://github.com/dolittle/Runtime/pull/500)
## Summary

We saw inconsistent issues while committing events in applications with event handlers that caused the runtime to deadlock when committing an event, never returning.

Inspired by this blog post https://devblogs.microsoft.com/premier-developer/the-danger-of-taskcompletionsourcet-class/ we have a theory that it might be because when a TaskCompletionSource is being waited for and SetResult is called on it then all the task's async continuations will be invoked synchronously which could lead to a deadlock scenario. If that's the case then this issue can be resolved by giving the TaskCompletionSource the TaskCreationOptions.RunContinuationsAsynchronously flag when created to force the continuations to be run asynchronously.

We have yet to be able to write tests that provoke this scenario. Until then we can never know whether the issue has been resolved or not, or that that was the problem to begin with. I've added a single spec that failed when TaskCreationOptions.RunContinuationsAsynchronously was not given to the TaskCompletionSource and passes when it is. Thus IMO validating this fix

### Changed

- Made EventWaiter more robust
- Added TaskCreationOptions.RunContinuationsAsynchronously to all manually created TaskCompletionSources

### Added
- A spec that tries provoke the dead-lock by calling Notify and Wait in different threads and call Notify (running in separate thread from the thread calling Wait) between two Wait-calls. In this scenario the continuation of the first Wait call would be started synchronously, running on the thread that called Notify, after the Notify called TrySetResult on the TaskCompletionSource that the Wait-call awaits. And since the "Notify-thread" has the readWrite lock we get a dead-lock when the next Wait-call tries to get this lock.

### Fixed

- A wrongly formatted log message when committing events.

# [5.4.1] - 2021-2-4 [PR: #499](https://github.com/dolittle/Runtime/pull/499)
## Summary

A problem could occur when commiting multiple events because in regards to eventual consitency. This should likely solve that issue by notifying the event after the transaction has been committed.

### Fixed

- Notify event waiter outside of the transaction

# [5.4.0] - 2021-2-4 [PR: #498](https://github.com/dolittle/Runtime/pull/498)
## Summary

First rough take on getting a RESTful API around the EventStore. Staying true to the gRPC protobuf contracts

### Added

- Rest endpoints around the event store in a controller
- /api/event/commit - Commits events
- /api/event/commitForAggregate - Commits aggregate events
- /api/event/fetchForAggregate - Fetch events for aggregate

### Changed

- Added a layer between the gRPC EventStore service and the Event Store which is a service that is a singleton per tenant. The new EventStore controller is also using it

# [5.3.5] - 2021-2-4 [PR: #497](https://github.com/dolittle/Runtime/pull/497)
## Summary

Fixes a bug with our new `ConceptAs` type, that when trying to fetch nonexistent aggregate events, we would get a NullPointerException due to the implicit operator  from `ConceptAs` not checking first for `null`.

### Fixed

- Fix the implicit conversion on `ConceptAs` to check first if it's `null`
- Fix the `ToString()` on `ConceptAs` to check first if it's `null`
- Fix the `AggregateRootVersion` when converting `CommittedAggregateEvents` to their Protobuf representation when it was `null`

# [5.3.4] - 2021-2-2 [PR: #487](https://github.com/dolittle/Runtime/pull/487)
## Summary

Remove unnecessary dependencies and rework some of the old fundamental parts of the code.

### Added
- `.editorconfig` for enforcing our rules and styles

### Changed

- Change to .NET 5
- Change old value type classes to be `record`s to utilize the cool new C# 9 `record` type
- Changed most of the logging to utilise LoggerMessage both for performance gains and for keeping log messages together

### Fixed

- Fixed a bug in a logging while filtering events
- Added calls to `GC.SuppressFinalize()` in Dispose() methods as per [CA1816](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1816)
- Many "constant" properties we're missing `readonly` keyword
- Fixed many `using` statements to the new simpler form
- Uses `var`s whenever possible now
- Start using `new` statements

### Removed

- Remove Concepts and Value types - replace with records
- Remove Serialisation of Concepts
- Our custom logging wrapper around Microsofts own.
- Removed Validation as it was unused
- Remove custom Scheduler abstraction - the Runtime will always run in a multi-core, multi-threaded linux environment so we don't need to apply abstractions to performing scheduling tasks.
- Remove abstraction around time
- Remove InitialSystems bootstage
- Remove dependency to Dolittle.Common and use `.editorconfig` instead
- `ILogger`s from many classes where it wasn't being used

# [5.3.3] - 2021-01-25
## Fixed
- Updated the dependencies related to Grpc (v2.35.0) and protobuf.

# [5.3.2] - 2021-01-22
## Fixed
- Merged dolittle/DotNet.Fundamentals into the Runtime for ease of development.

# [5.3.1] - 2021-01-19
## Fixed
- Reduced debug logging caused by waiting for new events for each stream processor.

# [5.3.0] - 2021-01-19
## Fixed
- Significantly reduced CPU usage by drastically decreasing the number of requests to the EventStore. Now the stream processors will get notified when an event is available to be fetched. Has a 1-minute timeout in case of problems.

# [5.2.0] - 2020-12-02
## Added
- Support for configuring port in EventStore MongoDB server addresses
- Support for configuring MaxConnectionPoolSize (default is 100)

# [5.1.4] - 2020-10-30
## Fixed
- When committing an event with expected aggregate root version to an aggregate root which has already been committed to it will now throw an appropriate error instead of timing out after 30 seconds.

# [5.1.3] - 2020-10-27
## Added
- More logging for aggregates

# [5.1.2] - 2020-10-11
## Fixed
- Fix JSON <-> BSON conversion in MongoDB event store

# [5.1.1] - 2020-10-06
## Fixed
- Fix CI pipeline for publishing dev docker image

# [5.1.0] - 2020-10-06
## Added
- Add dolittle/runtime:latest-development docker image

# [5.0.3] - 2020-09-26
## Fixed
- Fix EventHorizon Consents to be passed to Client when setting up a subscription

# [5.0.2] - 2020-09-01
## Fixed
- Fix committed events getting their proper EventSourceId's set

# [5.0.1] - 2020-07-09
## Added
- More logging
