// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.given;

public class all_dependencies
{
    protected static IDetectEmbeddingLoops detector;
    protected static Mock<ICompareStates> comparer;
    protected static IList<ProjectionState> previous_states;

    Establish context = () =>
    {
        previous_states = new List<ProjectionState>
        {
            CreateStateWithKeyValue("FirstObject", "FirstObjectValue"),
            CreateStateWithKeyValue("SecondObject", "SecondObjectValue"),
            CreateStateWithKeyValue("ThirdObject", "ThirdObjectValue"),
            CreateStateWithKeyValue("FourthObject", "FourthObjectValue"),
            CreateStateWithKeyValue("FifthObject", "FifthObjectValue"),
            CreateStateWithKeyValue("SixthObject", "SixthObjectValue")
        };

        comparer = new Mock<ICompareStates>();
        detector = new EmbeddingLoopsDetector(comparer.Object);
    };

    protected static ProjectionState CreateStateWithKeyValue(string key, string value)
    {
        var jObject = new JObject
        {
            { key, value }
        };
        return new ProjectionState(JsonConvert.SerializeObject(jObject));
    }
}