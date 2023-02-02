using System.Collections.Generic;
using System.Dynamic;

namespace Dolittle.Runtime.Configuration.Management;

/// <summary>
/// Defines a system that knows about all the Dolittle configurations.
/// </summary>
public interface IDolittleConfiguration
{
    ExpandoObject Config { get; }

    ExpandoObject Include(string includePath);
    
    ExpandoObject Ignoring(IEnumerable<string> ignores);
    

}