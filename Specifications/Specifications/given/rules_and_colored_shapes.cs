// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Specifications.given
{
    public class rules_and_colored_shapes : colored_shapes
    {
        protected static Specification<ColoredShape> red;
        protected static Specification<ColoredShape> blue;
        protected static Specification<ColoredShape> green;
        protected static Specification<ColoredShape> yellow;
        protected static Specification<ColoredShape> circles;
        protected static Specification<ColoredShape> squares;
        protected static Specification<ColoredShape> triangles;
        protected static Specification<ColoredShape> pentagons;

        Establish context = () =>
            {
                red = new ColorRule("Red");
                blue = new ColorRule("Blue");
                green = new ColorRule("Green");
                yellow = new ColorRule("Yellow");
                circles = new ShapeRule("Circle");
                squares = new ShapeRule("Square");
                triangles = new ShapeRule("Triangle");
                pentagons = new ShapeRule("Pentagon");
            };
    }
}