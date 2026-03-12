using System;
using System.Collections.Generic;
using Remind.Core.Util;

namespace Remind.Core.Commands
{
    public class RemindHelpCommand : IChatCommand
    {
        public const string CMD = "remindhelp";
        public string Name => CMD;
        public string ShortName => "rh";
        public string Description => "Lists all available commands.";

        Dictionary<string, IChatCommand>? commandsMap;
        public void Initialize(Dictionary<string, IChatCommand> commandsMap)
        {
            this.commandsMap = commandsMap;
        }

        public void Execute(string[] args)
        {
            if (commandsMap == null)
            {
                ChatUtils.AddGlobalNotification("No commands available!");
                return;
            }

            // Rebuild the sorted set each time so commands registered by sub-mods (e.g. RemindChatLog,
            // RemindTelegram) are always included without requiring a second Initialize call.
            var sorted = new SortedSet<IChatCommand>(commandsMap.Values);

            if (args.Length == 0)
            {
                ChatUtils.AddGlobalNotification($"Remind Version ({RemindPlugin.ModVersion}).");
                ChatUtils.AddGlobalNotification("For detailed usage of a specific command, type /remindhelp (/fh) [command].");
                ChatUtils.AddGlobalNotification("Available commands:");
                foreach (var cmd in sorted)
                {
                    ChatUtils.AddGlobalNotification($"/{cmd.Name} (/{cmd.ShortName})");
                }
            }

            if (args.Length == 1)
            {
                string commandName = args[0].ToLower();
                if (commandName == "verbose")
                {
                    ChatUtils.AddGlobalNotification($"Remind Version ({RemindPlugin.ModVersion}).");
                    ChatUtils.AddGlobalNotification("For detailed usage of a specific command, type /remindhelp (/fh) [command].");
                    ChatUtils.AddGlobalNotification("Available commands:");
                    foreach (var cmd in sorted)
                    {
                        ChatUtils.AddGlobalNotification($"/{cmd.Name} (/{cmd.ShortName}): {cmd.Description}");
                    }
                }
                else if (commandsMap.TryGetValue(commandName, out IChatCommand? cmd))
                {
                    ChatUtils.AddGlobalNotification($"/{cmd.Name} (/{cmd.ShortName}): {cmd.Description}");
                }
                else
                {
                    ChatUtils.AddGlobalNotification($"Command not found: {commandName}");
                }
            }
        }
    }
}
