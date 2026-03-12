using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using Remind.Core.Commands;
using Remind.Core.Util;

namespace Remind.Core
{
    public class ChatCommandManager
    {
        private readonly ManualLogSource _log = BepInEx.Logging.Logger.CreateLogSource($"{RemindPlugin.ModName}.CM");

        private readonly Dictionary<string, IChatCommand> _commands;

        public ChatCommandManager()
        {
            // Use reflection to find all types that implement IChatCommand and are not abstracts
            IEnumerable<Type> commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IChatCommand).IsAssignableFrom(t)
                         && !t.IsInterface
                         && !t.IsAbstract);

            // Load all injected commands into a fast-lookup dictionary
            this._commands = new Dictionary<string, IChatCommand>();
            foreach (var cmdType in commandTypes)
            {
                var cmd = Activator.CreateInstance(cmdType) as IChatCommand;
                Register(cmd);
            }

            // Initialize the help command with the full list of commands for dynamic help text
            InitializeHelpCommand(this._commands);
        }

        public void Register(IChatCommand? command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Name) || string.IsNullOrWhiteSpace(command.ShortName))
            {
                _log.LogWarning($"Attempted to register invalid command: {command?.GetType().Name ?? "null"}");
                return;
            }

            this._commands[command.Name] = command;
            this._commands[command.ShortName] = command;
            _log.LogInfo($"Registered command: /{command.Name} (/{command.ShortName})");
        }

        private static void InitializeHelpCommand(Dictionary<string, IChatCommand> commands)
        {
            if (commands.TryGetValue(RemindHelpCommand.CMD, out IChatCommand? helpCmd))
            {
                (helpCmd as RemindHelpCommand)?.Initialize(commands);
            }
        }

        public bool ProcessInput(string input)
        {
            // Split "/roll 2d20 d7" -> ["roll", "2d20", "d7"]
            if (!ChatCommandArgs.TryParse(input, out ChatCommandArgs? commandArgs))
            {
                // input not processed : not a valid command format
                return false;
            }

            // Look up command by name and execute if found
            if (_commands.TryGetValue(commandArgs.Name, out IChatCommand? command))
            {
                _log.LogInfo($"Executing command: {commandArgs}");
                try
                {
                    command.Execute(commandArgs.Args);
                }
                catch (Exception ex)
                {
                    ChatUtils.AddGlobalNotification($"Error executing '{commandArgs}': {ex.Message}");
                }
                // input processed
                return true;
            }
            else
            {
                // input not processed : no matching command found
                return false;
            }
        }
    }
}
