---
title: Failures
description: The known failures and their associated codes
weight: 100
---

## Event Store

| Code | Failure
|------|---------|
| b6fcb5dd-a32b-435b-8bf4-ed96e846d460 | Event Store Unavailable
| d08a30b0-56ab-43dc-8fe6-490320514d2f | Event Applied By Other Aggregate Root
| b2acc526-ba3a-490e-9f15-9453c6f13b46 | Event Applied To Other Event Source
| ad55fca7-476a-4f68-9411-1a3b087ab843 | Event Store Persistance Error
| 6f0e6cab-c7e5-402e-a502-e095f9545297 | Event Store Consistency Error
| eb508238-87ff-4519-a743-03be5196a83d | Event Store Sequence Is Out Of Order
| 45a811d9-bdf7-4ee1-b9bc-3f248e761799 | Event Cannot Be Null
| eb51284e-c7b4-4966-8da4-64a862f07560 | Aggregate Root Version Out Of Order
| f25cccfb-3ae1-4969-bee6-906370ffbc2d | Aggregate Root Concurrency Conflict
| ef3f1a42-9bc3-4d98-aa2a-942db7c56ac1 | No Events To Commit
