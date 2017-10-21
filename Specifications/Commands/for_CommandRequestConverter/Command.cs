using doLittle.Commands;

namespace doLittle.Runtime.Commands.Specs.for_CommandRequestConverter
{
    public class Command : ICommand
    {
        public string   StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
}
