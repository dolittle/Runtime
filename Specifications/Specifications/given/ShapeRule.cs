// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specifications.given;

public class ShapeRule : Specification<ColoredShape>
{
    readonly string _shape;

    public ShapeRule(string matchingShape)
    {
        _shape = matchingShape;
        Predicate = shape => shape.Shape == _shape;
    }
}