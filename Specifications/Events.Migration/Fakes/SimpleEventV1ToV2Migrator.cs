namespace doLittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventV1ToV2Migrator : IEventMigrator<SimpleEvent, v2.SimpleEvent>
    {
        public v2.SimpleEvent Migrate(SimpleEvent source)
        {
            var simpleEvent2 = new v2.SimpleEvent();
            return simpleEvent2;
        }
    }
}