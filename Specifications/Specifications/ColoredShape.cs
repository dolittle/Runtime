// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Specifications;

public class ColoredShape
{
    public string Shape { get; set; }

    public string Color { get; set; }

    public override string ToString()
    {
        return string.Format("{0}:{1}", Shape, Color);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var target = (ColoredShape)obj;
        return target.Shape == Shape && target.Color == Color;
    }

    public override int GetHashCode()
    {
        return Shape.GetHashCode() + Color.GetHashCode();
    }
}