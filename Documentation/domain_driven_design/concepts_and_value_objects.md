---
title: About Concepts and Value Objects
description: Learn about Concepts and Value Objects in Dolittle
keywords: domain-driven-design
author: einari, smithmx, tomasekeli
aliases:
    - /runtime/runtime/domain_driven_design/concepts_and_value_objects
---

# Background

Objects which have identity, fall into two distinct categories.  *Entities* and *Value Objects*

## Entities

*Entities* have a unique key, usually a single value but in some cases a composite of multiple values, that identifies the instance of the entity.  When comparing two instances it is sufficient to compare the identities of those entities.

An entity will have the same identifier throughout the lifetime of the system, even though it may be represented by different classes and objects hroughout its lifetime.  For example, an order in an ecommerce system may at different times be *PlacedOrder*, *PickingList*, *ShippedOrder*, *DelinquentOrder* and so on, but it will always be identified by the same *OrderId*.  In other words, it is the value of the key that determines the identity of the entity, not the particular class that currently represents it or the current state of that entity.

## Value Objects

*Value Objects* do not have any key that identifies them. A *Value Object* is identified by the properties that comprise it.  It follows that two instances of a value object, that have identical properties, are considered equal. Examples of value objects could be telephone numbers, a co-ordinate, an address, etc..  It should be pointed out that the distinction between entity and value object can be domain specific.  What is a value object in one domain, might be an entity in another domain.


# Primitive Obsession

The term *primitive obsession* is used to describe the tendency to use language-specific  primitives (strings, integers, doubles, etc.) to represent concepts in the domain, rather than fully-fledged types. While ultimately all concepts must be expressed in the primitive types available in the language writing the domain-code like this leads to a less expressive, less correct domain model. For example, a telephone number might be represented as a *string* (or even an integer).  However, there is logic associated with a telephone number and aspects of it that are not necessarily captured by a string.  We may be interested in area codes, international dialling codes, extensions, country specific formatting rules, and so on. Replacing a primitive with a type allows us to capture that while not all strings are telephone numbers, all telephone numbers can be expressed as strings.

{{% notice warning %}}
Never *ever* represent things that look like "numbers" by virtue of being composed solely of digits as integers. This is general advice that will take you far in your career.
{{% /notice %}} 

# Value Objects in Dolittle

Value object **MUST** be treated as immutable after creation and initial population.  Dolittle does not currently enforce immutability, primarily as a concession to serialization and testing concerns.  However, it is imperative that you do not alter any properties of a value object. Instead, you should always create and return a new instance.  Altering the properties of a Value Object will change the results of equality comparisons and the result of *GetHashCode()*. Plainly: changing the properties' values changes reality in inconsistent ways. A duck can now be a dog without having gone through a process to change, and may in fact oscillate between the two.

Dolittle provides two different artifacts to support the creation of objects with value-identity semantics.

# ConceptAs\<T>

*ConceptAs\<T>* is intended as a direct replacement for where you would use a single primitive to represent a concept in the domain.  As well as implementing equality checks, and producing hash codes, *ConceptAs\<T>* allows an (almost) seamless transformation between the underlying primitive and the domain concept. This has proven to be very useful at the boundaries of your application, where you may be indeed receiving primitives. It also aids in serialization and integration with other systems. *ConceptAs\<T>* implements an implicit casting between the *ConceptAs\<T>* and the primitive *T*.

A typical implementation of a ConceptAs might be

```C#
public class TelephoneNumber : ConceptAs<string>
{
    public static implicit operator TelephoneNumber(string value)
    {
        return new TelephoneNumber { Value = value };
    }
}
```

While the underlying ConceptAs\<T> class can handle the implicit conversion to a string, it is not possible to implement the conversion to the specific type (i.e. TelephoneNumber) in a generic manner in the base class. ConceptAs\<T> does support IComparable\<ConceptAs\<T>> and IComparable and also has overloaded comparison operators (`< > <= >=`). This makes it quite easy to replace the primitive with a ConceptAs\<T> while keeping code using the concept the same. You may, however, get compile-errors if you are misusing the power of primitives (for example by multiplying telephone numbers together).

It is **NOT RECOMMENDED** to enforce *correctness* of a ConceptAs\<T> by throwing an exception if there is an attempt to create an instance in an invalid state.  It is the preferred strategy in Dolittle to capture invalid state through the mechanism of input validation. This allows a more graceful and  informative handling of the invalid state.

{{% notice tip %}}  
Use of ConceptAs\<T> can lead to loss of type inference in e.g. lambda functions.  It can also require a little more work on the part of the developer when serializing.  We consider this a good trade-off given the explicitness and expressiveness it brings to your domain.
{{% /notice %}}  

# Value\<T>

*Value\<T>* provides basic value-object semantics by handling equality and production of hash codes.  When a class *T* inherits from *Value\<T>*, then equality and hash code will depend on the values of all public properties.  In contrast to *ConceptAs\<T>*, *Value\<T>* is intended to group properties that belong together into a domain concept.  For example, a `Money : Value<Money>` might combine a monetary amount with a currency.  A `Point : Value<Point>` might have properties for the X, Y and Z axis.

It is *RECOMMENDED* and *ENCOURAGED* to use *ConceptAs\<T>* as properties within *Value\<T>*.  It is  *DISCOURAGED* but *NOT PROHIBITED* from using *Value\<T>* with just a single property.

{{% notice tip %}}  
Value objects within Dolittle consider the *System Type* to be integral to their identity.  While it is permissible to inherit from a Value\<T> or ConceptAs\<T> that you have created, a type of the base class and a type of subclass **WILL NOT** be considered equal, even if they have all properties identical.  A value object can only be equal to itself or another object of the same type.
{{% /notice %}}  

*Value\<T>* is not automatically comparable like ConceptAs\<T>. There is nothing stopping you from implementing it, but there is no natural, default way to compare *Value\<T>*. Comparison is therefor left to the implementor.

