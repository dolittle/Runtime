# Title
Event Handler Throughput

## Status

Proposed

## Context

The current implementation of event handlers have a throughput which is limited by the latency between the runtime and SDK.
Each event is processed, and the result is committed and handled internally in terms of updating state in the database before the next handler is fired.
This limits the theoretical max throughput of the system.

Since we are integrating external systems with millions of records, the need for high throughput is becoming apparent. This will let both the initial import from the remote system and later read model population happen quickly.



## Decision

In order to process on the order of many thousands of events / sec, we can restructure how we process them in the SDK / runtime.



What is the change that we're proposing and/or doing?

## Consequences

What becomes easier or more difficult to do because of this change?
