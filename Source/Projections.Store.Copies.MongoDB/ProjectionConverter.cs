// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionConverter"/> class.
    /// </summary>
    /// <param name="converter">The converter to use for converting values.</param>
    public ProjectionConverter(IValueConverter converter)
    {
        _converter = converter;
    }

    /// <inheritdoc />
    public BsonDocument Convert(ProjectionState state, IDictionary<ProjectionField, ConversionBSONType> conversions)
    {
        var document = BsonDocument.Parse(state);
        
        foreach (var (field, conversion) in conversions)
        {
            ConvertField(document, field, conversion);
        }

        return document;
    }

    void ConvertField(BsonDocument document, ProjectionField field, ConversionBSONType conversion)
    {
        foreach (var (value, setter) in GetFieldValuesAndSetters(document, field))
        {
            var newValue = _converter.Convert(value, conversion);
            setter(newValue);
        }
    }

    static IEnumerable<ValueAndSetter> GetFieldValuesAndSetters(BsonDocument document, ProjectionField field)
        => GetFieldPath(field)
            .Aggregate<string, IEnumerable<ValueAndSetter>>(
                new ValueAndSetter[] { new(document, _ => { }) },
                GetPropertyValuesAndSetters);

    static IEnumerable<ValueAndSetter> GetPropertyValuesAndSetters(IEnumerable<ValueAndSetter> parents, string property)
        => parents.SelectMany(valueAndSetter =>
        {
            var parent = valueAndSetter.Value;
            if (parent is not BsonDocument document)
            {
                throw new ValueIsNotDocument(parent.BsonType, property);
            }

            if (!document.Contains(property))
            {
                throw new DocumentDoesNotHaveProperty(document, property);
            }

            var value = document.GetValue(property);
            if (value is not BsonArray array)
            {
                return new ValueAndSetter[] {new(value, newValue => document.Set(property, newValue))};
            }

            return array.Select((element, index) => new ValueAndSetter(element, newValue => array[index] = newValue)).ToArray();
        });
    
    static IEnumerable<string> GetFieldPath(ProjectionField field)
        => field.Value.Split('.').Select(_ => _.Replace("\\.", "."));
}
