using MongoDB.Bson.IO;

namespace Integration.Tests.Events.Processing.given;

static class event_content_serializer
{
    public static readonly JsonWriterSettings json_settings = new()
    {
        OutputMode = JsonOutputMode.Strict,
        Indent = false,
    }; 
}