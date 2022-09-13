// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integration.Tests.Events.Store.given;

static class event_to_commit
{
    
    public static UncommittedEvent create() => with_content(new {Hello = 42}).build();
    public static UncommittedEvent create_with_type(ArtifactId event_type) => with_content(new {Hello = 42}).with_event_type(event_type).build();

    public static event_builder with_content(object content) => new(content);
    
    public class event_builder
    {
        EventSourceId event_source_id = "some event source";
        Artifact event_type = new("a52f686f-c045-4c7a-9a5b-91ee4b107237", ArtifactGeneration.First);
        bool is_public;
        readonly string content;
        
        public event_builder(object content)
        {
            this.content = JsonConvert.SerializeObject(content, Formatting.None);
        }

        public event_builder public_event()
        {
            is_public = true;
            return this;
        }
        
        public event_builder with_event_type(ArtifactId artifact)
        {
            event_type = event_type with {Id = artifact};
            return this;
        }
        
        public event_builder with_event_source(EventSourceId event_source)
        {
            event_source_id = event_source;
            return this;
        }

        public UncommittedEvent build() => new(event_source_id, event_type, is_public, content);
    }
}