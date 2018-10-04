---
title: Convention over Configuration
description: Learn about how Dolittle looks at convention over configuration and vice versa
keywords: Runtime
author: einari
---

For a team to deliver consistency in the codebase, one should aim for a recipe that makes it easy to the right thing and
hard to the wrong thing. Most software projects tend to put in place both formal rules and informal rules in the codebase
that are recipes considered good for the project. These are often aimed at increasing the productivity of the developer and
making it more predictable where and how to do things.

These recipes are often not formalized to the level of being enforced by something like build time tools.
During times of high pressure and stress, the rules can easily be broken due to the "in the interest of time"-principle.
This is obviously something that makes the codebase inconsistent and hard to maintain over time.

At Dolittle we believe in formalizing the conventions, putting it front and center and making it hard to the wrong thing.
This helps in governing the recipes and forces the team to deliver more consistently and stripping away the ability to
take shortcuts. Dolittle itself throughout is relying heavily on this and enforcing some of the things hard while other things
might be more configurable conventions.

## Structure

An example of an enforced convention is how Dolittle looks at application structure. It expects an application with one or more
bounded context in it and within a bounded context you can have modules which in turn can have features in it. This hierarchy
gives the developers a vocabulary at the same time a strong structure. For instance, a C# developer will not be allowed to put
things in arbitrary namespaces as the Dolittle tooling is expecting artifacts to be in certain expected places based on the
topology of the bounded context.

## Discovering

A very strong principle at the heart of Dolittle is the notion of discovery. The code that we write can all be discovered either
at build-time or at runtime. This is a very powerful concept enabling one to much easier adhere to the [Open / Closed principle](https://en.wikipedia.org/wiki/Open–closed_principle).
Meaning that we don't have to open up code to get new things in place, it will be discovered by how the conventions are and configured.

You find examples of this in the Dolittle SDKs where things like event processors and command handlers are discovered and automatically
registered for the concrete command to be handled or event to be processed.

{{% notice tip %}}
If you're a C# developer, there are great ways to discover either [implementations of types](https://github.com/dolittle/DotNET.Fundamentals/blob/master/Source/Types/IImplementationsOf.cs) or [instances of types](https://github.com/dolittle/DotNET.Fundamentals/blob/master/Source/Types/IInstancesOf.cs).

These can simply be taken dependencies on in a constructor and you can start iterating over them.
{{% /notice %}}

## Inversion of Control conventions

The [inversion of control](https://en.wikipedia.org/wiki/Inversion_of_control) principle speaks about inverting the control of creation
and lifecycle of objects and leaving this responsibility to a container that governs this. A container holds binding definitions
internally that holds information about which concrete implementation to provide and which lifecycle it should be in
(transient, per thread, singleton and so forth). These definitions could be setup manually, putting a lot of responsibility
on the developer to do this manually and might even be error-prone. Often the binding is actually a recipe - a pattern the developers
do all the time, something that can be automated by leveraging discovery mechanism. For instance a very common pattern is to represents
the dependencies as `interfaces`. Concrete implementations are often then named almost exactly the same, only differing with a prefix
or a postfix. The pattern is then consistently applied and is something that can be discovered and bindings can be put in place
automatically. Example of this is an `interface` called `IFoo` bound towards `Foo`.

Another benefit of doing ths definitions programatically through discovery mechanism is the ability to then apply something like
lifecycle cross-cuttingly throughout the system.
