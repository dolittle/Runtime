// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.given;

public class a_renamer_and_inputs
{
    protected static PropertyRenamer property_renamer;

    protected static BsonDocument document_to_rename;
    protected static PropertyConversion[] conversions_to_apply;
    
    Establish context = () =>
    {
        property_renamer = new PropertyRenamer();

        conversions_to_apply = Array.Empty<PropertyConversion>();
    };

    protected static BsonDocument result;
    protected static Exception exception;

    Because of = () => exception = Catch.Exception(() => result = property_renamer.RenamePropertiesIn(document_to_rename, conversions_to_apply));
}