// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

record ValueAndSetter(BsonValue Value, Action<BsonValue> Setter);

/// <summary>
/// Represents an implementation of <see cref="IProjectionConverter"/>.
/// </summary>
public class ProjectionConverter : IProjectionConverter
{
    readonly IValueConverter _converter;
    readonly IPropertyRenamer _renamer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionConverter"/> class.
    /// </summary>
    /// <param name="converter">The converter to use for converting values.</param>
    /// <param name="renamer">The renamer to use for renaming properties.</param>
    public ProjectionConverter(IValueConverter converter, IPropertyRenamer renamer)
    {
        _converter = converter;
        _renamer = renamer;
    }

    /// <inheritdoc />
    public BsonDocument Convert(ProjectionState state, PropertyConversion[] conversions)
    {
        var document = BsonDocument.Parse(state);
        ConvertPropertiesIn(document, conversions);
        return _renamer.RenamePropertiesIn(document, conversions);
    }

    void ConvertPropertiesIn(BsonDocument document, PropertyConversion[] conversions)
    {
        foreach (var conversion in conversions)
        {
            ConvertPropertyIn(document, conversion);
        }
    }

    void ConvertPropertyIn(BsonDocument document, PropertyConversion conversion)
    {
        if (!document.Contains(conversion.Property))
        {
            throw new DocumentDoesNotHaveProperty(document, conversion.Property);
        }

        var value = document.GetValue(conversion.Property);

        if (value is BsonArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                ConvertChildrenPropertiesIn(array[i], conversion);

                if (conversion.Conversion == ConversionBSONType.None)
                {
                    continue;
                }
                
                var newElement = _converter.Convert(array[i], conversion.Conversion);
                array[i] = newElement;
            }
        }
        else
        {
            ConvertChildrenPropertiesIn(value, conversion);

            if (conversion.Conversion == ConversionBSONType.None)
            {
                return;
            }
            
            var newValue = _converter.Convert(value, conversion.Conversion);
            document.Set(conversion.Property, newValue);
        }
    }

    void ConvertChildrenPropertiesIn(BsonValue parent, PropertyConversion conversion)
    {
        if (conversion.Children.Length < 1)
        {
            return;
        }

        if (parent is not BsonDocument document)
        {
            throw new ValueIsNotDocument(parent.BsonType, conversion.Property);
        }
        
        ConvertPropertiesIn(document, conversion.Children);
    }
}
