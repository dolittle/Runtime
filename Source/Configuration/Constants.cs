using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration;

public static class Constants
{
    public static readonly string DolittleConfigSectionRoot = ConfigurationPath.Combine("dolittle", "runtime");

    public static string CombineWithDolittleConfigRoot(params string[] sections)
        => ConfigurationPath.Combine(sections.Prepend(DolittleConfigSectionRoot));
}
