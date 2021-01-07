---
title: Aggregates
description: Overview of Aggregates
weight: 70
---

An Aggregate is [Domain-driven design](https://en.wikipedia.org/wiki/Domain-driven_design) (DDD) term coined by Eric Evans. An aggregate is a collection of objects and it represents a concept in your domain, it's not a container for items. It's bound together by an Aggregate Root, which upholds the rules (invariants) to keep the aggregate consistent. It encapsulates the domain objects, enforces business rules, and ensures that the aggregate can't be put into an invalid state.

## Example

<!-- an aggregate tutorial would be so much better, maybe just scrap this part? -->

For example, in the domain of a restaurant, a `Kitchen` could be an aggregate, where it has domain objects like `Chefs`, `Inventory` and `Menu` and an operation `PrepareDish`.

The kitchen would make sure that:
- A `Dish` has to be on the `Menu` for it to be ordered
- The `Inventory` needs to have enough ingredients to make the `Dish`
- The `Dish` gets assigned to an available `Chef`

Here's a simple C#_ish_ example of what this aggregate root could look like:
```csharp
public class Kitchen
{
    Chefs _chefs;
    Inventory _inventory;
    Menu _menu;

    public void PrepareDish(Dish dish)
    {
        if (!_menu.Contains(dish))
        {
            throw new DishNotOnMenu(dish);
        }
        foreach (var ingredient in dish.ingredients)
        {
            var foundIngredient = _inventory
                .GetIngredient(ingredient.Name);
            if (!foundIngredient)
            {
                throw new IngredientNotInInventory(ingredient);
            }

            if (foundIngredient.Amount < ingredient.Amount)
            {
                throw new InventoryOutOfIngredient(foundIngredient);
            }
        }
        var availableChef = _chefs.GetAvailableChef();
        if (!availableChef)
        {
            throw new NoAvailableChefs();
        }
        availableChef.IsAvailable = false;
    }
}
```

## Aggregates in Dolittle

With [Event Sourcing]({{< ref "event_sourcing" >}}) the aggregates are the key components to enforcing the business rules and the state of domain objects. Dolittle has a concept called `AggregateRoot` in the [Event Store]({{< ref "event_store" >}}) that acts as an aggregate root to the `AggregateEvents` committed to it. Each aggregate event is _applied_ to the aggregate root so that it 

### Structure of an `AggregateRoot`

This is a simplified structure of the main parts of an aggregate root.
```csharp
AggregateRoot {
    EventSourceId Guid
    AggregateRootId Guid
    Version int
    AggregateEvents AggregateEvent[]
}
```

#### `EventSourceId`

`EventSourceId` represents the source of the event like a "primary key" in a traditional database. By default, [partitioned event handlers]({{< ref "event_handlers_and_filters#event-handlers" >}}) use it for [partitioning]({{< ref "streams.md#partitions" >}}).

##### `AggregateRootId`

Identifies this specific aggregate root.

#### `Version`

`Version` is the position of the next `AggregateEvent` to be processed. It's incremented after each `AggregateEvent` has been applied by the `AggregateRoot`.


## Designing aggregates

<!-- do we really ned to tal about aggregates? or just link to other places. aggregates are optional after all... -->

There are a few rules for using aggregates:
1. It has to have a single root/parent named after the purpose of the aggregate as a whole.
2. You cannot ask the aggregate any questions (no public properties, no query methods).
3. You cannot access any of its internals (children, properties)
4. An aggregate cannot contain another aggregate, only a link/reference.

### Aggregates rules

An aggregate defines which rules must succeed or fail together. There are 2 categories of rules:
1. Immediate rules: they have to be correct immediately and they can never be broken. They have concurrency and performance implications.
2. Eventual Rules: they can be broken for a while, but they will eventually be enforced.

Figuring out what kind of rules to put in place depends wholly on the domain. You should ask yourself: _"What is the impact of breaking this rule?" What happens in the domain if this rule is broken? Remember, that aggregates are not a technical concern, they are a domain concern.

## Further reading

- [Aggregates by Martin Fowler](https://martinfowler.com/bliki/DDD_Aggregate.html)
- [Aggregates in domain Driven Design](https://medium.com/ingeniouslysimple/aggregates-in-domain-driven-design-5aab3ef9901d)
- [Uncovering Hidden Business Rules with DDD Aggregates by Nick Tune](https://medium.com/nick-tune-tech-strategy-blog/uncovering-hidden-business-rules-with-ddd-aggregates-67fb02abc4b)
