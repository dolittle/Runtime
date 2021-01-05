---
title: Tenants
description: What is a Tenant & Multi-tenancy
weight: 13
---

Dolittle supports having multiple tenants using the same software out of the box.

## What is a Tenant?

A Tenant is a single client that's using the hosted software and infrastructure. In a SaaS (Software-as-a-Service) domain, a tenant would usually be a single customer using the service. The tenant has its privileges and resources only it has access to.

## What is Multi-tenancy?

In a multi-tenant application, the same instance of the software is used to serve multiple tenants. An example of this would be an e-commerce SaaS. The same basic codebase is used by multiple different customers, each who has their own customers and their own data.

Multi-tenancy allows for easier scaling, sharing of infrastructure resources, and easier maintenance and updates to the software.

![Simple explanation of multi tenancy](/images/concepts/multitenant-explanation.png)

## Multi-tenancy in Dolittle

In Dolittle, every tenant in a [Microservice]({{< ref "overview#Microservice" >}}) is identified by a GUID. Each tenant has their own [Event Store]({{< ref "event_store" >}}), managed by the [Runtime]({{< ref "overview#components" >}}). These event stores are defined in the Runtime [configuration files]({{< ref "docs/reference/runtime/configuration" >}}). The tenants all share the same Runtime, which is why you need to specify the tenant which to connect to when using the SDKs.
