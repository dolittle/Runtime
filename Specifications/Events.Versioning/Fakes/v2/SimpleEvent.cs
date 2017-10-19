namespace doLittle.Runtime.Events.Versioning.Specs.Fakes.v2
{
    public class SimpleEvent : Fakes.SimpleEvent, IAmNextGenerationOf<Fakes.SimpleEvent>
    {
        public static string DEFAULT_VALUE_FOR_SECOND_GENERATION_PROPERTY = "2nd: DEFAULT";

        public string SecondGenerationProperty { get; set; }

        public SimpleEvent()
        {
            SecondGenerationProperty = DEFAULT_VALUE_FOR_SECOND_GENERATION_PROPERTY;
        }
    }
}