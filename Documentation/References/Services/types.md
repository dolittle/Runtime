---
title: Types
description: The definition of the common types shared between Runtime services
weight: 1
---

<div class="mermaid">
classDiagram
    class CallRequestContext{
        ExecutionContext executionContext
        Uuid headId
    }
    class ExecutionContext{
        Uuid microserviceId
        Uuid tenantId
        Version version
        Uuid correlationid
        Claim[] claims
        string environment
    }
    class Version{
        int32 major
        int32 minor
        int32 patch
        int32 build
        string preReleaseString
    }
    class Claim{
        string key
        string value
        string valueType
    }
    class Failure{
        Uuid id
        string reason
    }
    class Artifact{
        Uuid id
        uint32 generation
    }
    CallRequestContext --* ExecutionContext
    ExecutionContext --o Version
    ExecutionContext --o Claim
</div>
