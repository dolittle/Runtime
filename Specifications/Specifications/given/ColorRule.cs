// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specifications.given;

public class ColorRule : Specification<ColoredShape>
{
    readonly string _Color;

    public ColorRule(string matchingColor)
    {
        _Color = matchingColor;
        Predicate = shape => shape.Color == _Color;
    }
}