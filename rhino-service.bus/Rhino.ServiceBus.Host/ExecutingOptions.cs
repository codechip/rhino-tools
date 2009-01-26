using System.Text;
using CommandLine;

namespace Rhino.ServiceBus.Host
{
    public class ExecutingOptions
    {
        [Argument(ArgumentType.Required, HelpText = "Choose an action", ShortName = "action")] public Action Action;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Assembly to execute", ShortName = "asm")] public string Assembly;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Configuration file", ShortName = "config")] public string ConfigFile;

        [Argument(ArgumentType.Required, HelpText = "Service name", ShortName = "name")] public string Name;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Should use loader", ShortName = "load-balancer")] public bool UseLoadBalancer;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(" /action:").Append(Action)
                .Append(" /load-balancer:")
                .Append(UseLoadBalancer ? "+" : "-")
                .Append(" /name:")
                .Append(Name);

            if (string.IsNullOrEmpty(Assembly) == false)
            {
                sb.Append(" /asm:")
                    .Append(Assembly);
            }
            if (string.IsNullOrEmpty(ConfigFile) == false)
            {
                sb.Append(" /cfg:")
                    .Append(ConfigFile);
            }
            return sb.ToString();
        }
    }
}