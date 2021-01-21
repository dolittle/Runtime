// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Dolittle.Build.MSBuild.Tasks
{
#pragma warning disable CA1819 // Allow arrays for properties
    /// <summary>
    /// Represents a task that is capable of discovering plugins to the Dolittle build pipeline.
    /// </summary>
    public class PluginAndConfigurationDiscoverer : Task
    {
        /// <summary>
        /// Gets or sets the aggregated plugins.
        /// </summary>
        [Required]
        public ITaskItem[] Plugins { get; set; }

        /// <summary>
        /// Gets or sets the path to the configuration file.
        /// </summary>
        [Output]
        public string ConfigurationFile { get; set; }

        /// <summary>
        /// Gets or sets the paths to assemblies that holds plugins.
        /// </summary>
        [Output]
        public string[] PluginAssemblies { get; set; }

        /// <inheritdoc/>
        public override bool Execute()
        {
            var pluginAssemblies = new List<string>();

            ConfigurationFile = Path.GetTempFileName();

            Log.LogMessage(MessageImportance.High, $"Plugin configurations will be stored in '{ConfigurationFile}'");

            var builder = new StringBuilder();
            var firstItem = true;
            builder.Append("{");
            foreach (var item in Plugins)
            {
                if (!firstItem) builder.Append(",");
                firstItem = false;

                var plugin = item.ItemSpec;

                var customMetadata = item.CloneCustomMetadata();
                builder.Append($"\"{plugin}\":{{");

                var pluginAssembly = GatherAllConfigKeys(builder, customMetadata);
                if (string.IsNullOrEmpty(pluginAssembly))
                {
                    Log.LogError($"Missing plugin assembly for plugin '{plugin}'");
                    return false;
                }

                pluginAssemblies.Add(pluginAssembly);
                builder.Append("}");
            }

            builder.Append("}");

            File.WriteAllText(ConfigurationFile, builder.ToString());

            PluginAssemblies = pluginAssemblies.Distinct().ToArray();
            return true;
        }

        string GatherAllConfigKeys(StringBuilder builder, IDictionary customMetadata)
        {
            var firstPair = true;
            var pluginAssembly = string.Empty;
            foreach (var key in customMetadata.Keys)
            {
                if (!firstPair) builder.Append(",");

                if (key.ToString() == "Assembly")
                {
                    pluginAssembly = customMetadata[key].ToString();
                }
                else
                {
                    firstPair = false;
                    var value = customMetadata[key].ToString();

                    var escapedValue = EscapeString(value);
                    builder.Append($"\"{key}\":\"{escapedValue}\"");
                }
            }

            return pluginAssembly;
        }

        bool NeedEscape(string src, int i)
        {
            char c = src[i];
            return c < 32 || c == '"' || c == '\\'

                // Broken lead surrogate
                ||
                (c >= '\uD800' && c <= '\uDBFF' &&
                    (i == src.Length - 1 || src[i + 1] < '\uDC00' || src[i + 1] > '\uDFFF'))

                // Broken tail surrogate
                ||
                (c >= '\uDC00' && c <= '\uDFFF' &&
                    (i == 0 || src[i - 1] < '\uD800' || src[i - 1] > '\uDBFF'))

                // To produce valid JavaScript
                ||
                c == '\u2028' || c == '\u2029'

                // Escape "</" for <script> tags
                ||
                (c == '/' && i > 0 && src[i - 1] == '<');
        }

        string EscapeString(string src)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int start = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (NeedEscape(src, i))
                {
                    sb.Append(src, start, i - start);
                    switch (src[i])
                    {
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        case '\"':
                            sb.Append("\\\"");
                            break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '/':
                            sb.Append("\\/");
                            break;
                        default:
                            sb.Append("\\u");
                            sb.Append(((int)src[i]).ToString("x04", CultureInfo.InvariantCulture));
                            break;
                    }

                    start = i + 1;
                }
            }

            sb.Append(src, start, src.Length - start);
            return sb.ToString();
        }
    }
#pragma warning restore CA1819 // Allow arrays for properties
}