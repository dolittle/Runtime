# [6.2.2] - 2022-68
## Summary

Back porting of public filter `OutOfMemoryException` fix

### Fixed

- Fixes a problem with the public filter where it could throw an `OutOfMemoryException` if the event log was big enough

# [6.2.1] - 2022-5-3
## Summary

Back porting of readiness health checks to v6 Runtime

### Fixed

- Fixes a problem with the event horizon producer where it will continuously poll a public stream for an event that is not present

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
