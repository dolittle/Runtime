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
