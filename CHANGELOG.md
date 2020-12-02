# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.2.0] - 2020-12-02
### Added
- Support for configuring port in EventStore MongoDB server addresses
- Support for configuring MaxConnectionPoolSize (default is 100)

## [5.1.4] - 2020-10-30
### Fixed
- When committing an event with expected aggregate root version to an aggregate root which has already been committed to it will now throw an appropriate error instead of timing out after 30 seconds.

## [5.1.3] - 2020-10-27
### Added
- More logging for aggregates

## [5.1.2] - 2020-10-11
### Fixed
- Fix JSON <-> BSON conversion in MongoDB event store

## [5.1.1] - 2020-10-06
### Fixed
- Fix CI pipeline for publishing dev docker image

## [5.1.0] - 2020-10-06
### Added
- Add dolittle/runtime:latest-development docker image

## [5.0.3] - 2020-09-26
### Fixed
- Fix EventHorizon Consents to be passed to Client when setting up a subscription

## [5.0.2] - 2020-09-01
### Fixed
- Fix committed events getting their proper EventSourceId's set

## [5.0.1] - 2020-07-09
### Added
- More logging

