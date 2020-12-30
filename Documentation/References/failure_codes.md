---
title: Failures
description: The known failures and their associated codes
weight: 100
---

## Event Store

| Code | Failure
|------|---------|
| `b6fcb5dd-a32b-435b-8bf4-ed96e846d460` | Event Store Unavailable
| `d08a30b0-56ab-43dc-8fe6-490320514d2f` | Event Applied By Other Aggregate Root
| `b2acc526-ba3a-490e-9f15-9453c6f13b46` | Event Applied To Other Event Source
| `ad55fca7-476a-4f68-9411-1a3b087ab843` | Event Store Persistance Error
| `6f0e6cab-c7e5-402e-a502-e095f9545297` | Event Store Consistency Error
| `eb508238-87ff-4519-a743-03be5196a83d` | Event Store Sequence Is Out Of Order
| `45a811d9-bdf7-4ee1-b9bc-3f248e761799` | Event Cannot Be Null
| `eb51284e-c7b4-4966-8da4-64a862f07560` | Aggregate Root Version Out Of Order
| `f25cccfb-3ae1-4969-bee6-906370ffbc2d` | Aggregate Root Concurrency Conflict
| `ef3f1a42-9bc3-4d98-aa2a-942db7c56ac1` | No Events To Commit

## Filters

| Code | Failure
|------|---------|
| `d6060ba0-39bd-4815-8b0e-6b43b5f87bc5` | No Filter Registration Received
| `2cdb6143-4f3d-49cb-bd58-68fd1376dab1` | Cannot Register Filter Or Non Writeable Stream
| `f0480899-8aed-4191-b339-5121f4d9f2e2` | Failed To Register Filter

## Event Handlers

| Code | Failure
|------|---------|
| `209a79c7-824c-4988-928b-0dd517746ca0` | No Event Handler Registration Received
| `45b4c918-37a5-405c-9865-d032869b1d24` | Cannot Register Event Handler Or Non Writeable Stream
| `dbfdfa15-e727-49f6-bed8-7a787954a4c6` | Failed To Register Event Handler

## Event Horizon

| Code | Failure
|------|---------|
| `9b74482a-8eaa-47ab-ac1c-53d704e4e77d` | Missing Microservice Configuration
| `a1b791cf-b704-4eb8-9877-de918c36b948` | Did Not Receive Subscription Response
| `2ed211ce-7f9b-4a9f-ae9d-973bfe8aaf2b` | Subscription Cancelled
| `be1ba4e6-81e3-49c4-bec2-6c7e262bfb77` | Missing Consent
| `3f88dfb6-93d6-40d3-9d28-8be149f9e02d` | Missing Subscription Arguments
