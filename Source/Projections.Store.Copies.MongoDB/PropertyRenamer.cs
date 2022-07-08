// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IPropertyRenamer"/>.
/// </summary>
public class PropertyRenamer : IPropertyRenamer
{
    /// <inheritdoc />
    public BsonDocument RenamePropertiesIn(BsonDocument document, PropertyConversion[] conversions)
    {
        var newDocument = new BsonDocument();

        CopyUnchangedProperties(document, newDocument, conversions);
        CopyRenamedProperties(document, newDocument, conversions);
        
        return newDocument;
    }

    void CopyUnchangedProperties(BsonDocument oldDocument, BsonDocument newDocument, PropertyConversion[] conversions)
    {
        foreach (var element in oldDocument)
        {
            if (!Array.Exists(conversions, conversion => element.Name == conversion.Property.Value))
            {
                newDocument.Set(element.Name, element.Value);
            }
        }
    }
    
    void CopyRenamedProperties(BsonDocument oldDocument, BsonDocument newDocument, PropertyConversion[] conversions)
    {
        foreach (var conversion in conversions)
        {
            if (!oldDocument.Contains(conversion.Property))
            {
                throw new DocumentDoesNotHaveProperty(oldDocument, conversion.Property);
            }

            var value = oldDocument.GetValue(conversion.Property);
            var renamedValue = RenameChildrenPropertiesIn(value, conversion);

            var name = conversion.ShouldRename ? conversion.RenameTo : conversion.Property;

            if (newDocument.Contains(name))
            {
                throw new DocumentAlreadyContainsProperty(newDocument, name);
            }
            
            newDocument.Set(name, renamedValue);
        }
    }

    BsonValue RenameChildrenPropertiesIn(BsonValue value, PropertyConversion conversion)
    {
        if (conversion.Children.Length < 1)
        {
            return value;
        }

        if (value is BsonArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                if (array[i] is not BsonDocument element)
                {
                    throw new ValueIsNotDocument(array[i].BsonType, conversion.Property);
                }

                array[i] = RenamePropertiesIn(element, conversion.Children);
            }

            return array;
        }

        if (value is not BsonDocument document)
        {
            throw new ValueIsNotDocument(value.BsonType, conversion.Property);
        }

        return RenamePropertiesIn(document, conversion.Children);
    }
}
