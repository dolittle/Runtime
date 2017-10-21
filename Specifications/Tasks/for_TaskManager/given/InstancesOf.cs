using System.Collections.Generic;

namespace doLittle.Specs.Tasks.for_TaskManager.given
{
    public class InstancesOf<T> : List<T>, Types.IInstancesOf<T>
        where T:class
    {
        public InstancesOf(IEnumerable<T> items) : base(items)
        {

        }
    }
}