---
title: Artifacts
description: Introduction to Artifacts in dolittle
keywords: artifact, migration, identity  
author: smithmx
aliases:
    - /runtime/runtime/artifacts
---

# Artifacts

## Definition
An Artifact is a representation in our domain that we wish to evolve through time independently of any particular code type that it may have been expressed in. 

## Introduction
An Event Sourced system makes migrations and versioning explicit, rather than implicit or one-off data migrations. However, in terms of code representations (classes, structs, interfaces, etc.), the read-only, append-only nature of persistence coupled with the need for a long-lived evolving system, can present challenges.

Simple refactorings such as renaming of a class or property or adding new a new property can cause issues when trying to rehydate an instance from a serialized instance of an earlier representation.  More importantly however, a domain model is an ongoing learning experience where we should always be looking to **refactor to deeper insight** in the phrase of Eric Evans in **Domain Driven Design**.  This simply means that as we learn more about our domain, we want our model to evolve to represent this deeper knowledge and we do not want to be inhibited by issues connected to serialization, code artifacts, etc.

## Separating code representation from concept
One way to deal with the issue of an evolving code representation is to maintain all previous code types (classes, structs, etc.) and version through some type of annotation or convention system.  This however soon leads to code bloat and the pollution of our domain language with postfixes such as "version".  It is not a scalable solution.

A better approach is to separate the concept that a code type represents from any particular code type representation. This concept is what we refer to as an *Artifact*.  

At its simplest, an **Artifact** is simply a unique indentifier.  This is enough to give the concept as identity that can span any particular code type.  In addition, every artifact has a version which is referred to as the **Generation**, which is an incremental number for each *version* of the artifact that the system has had.

##  Artifacts
There are a number of artifacts that we wish to evolve:
* Commands
* Events
* Event Sources

Other artifact-like entities that we wish to identify and that may a long-lived existence but do not require migration
* Queries
* Read Models
* Event Processors

## Metadata
Artifacts are tracked via metadata and reflection.  This allows us to indentify new artifacts and new versions of existing artifacts and enforce rules such as providing migrators between generations.  It also gives insight into the structure of our application.

## Migrators
[to be added]