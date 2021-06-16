---
title: Event Store
description: The definition of the Runtime Event Store service
weight: 1
---

## Message types
<div class="mermaid">
 classDiagram
      class Duck{
          +String beakColor
          +swim()
          +quack()
      }
      class Fish{
          -int sizeInFeet
          -canEat()
      }
      class Zebra{
          +bool is_wild
          +run()
      }
</div>

## Commit events
<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: CommitEventsRequest
    activate R
    R->>C: CommitEventsResponse
    deactivate R
</div>
