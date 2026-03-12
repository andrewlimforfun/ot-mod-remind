using System;
using Remind;
using Remind.Core.Util;

namespace Remind.Core.Commands
{
    public class RemindGlobalCommand : IChatCommand
    {
        public const string CMD = "remindglobal";
        public string Name => CMD;
        public string ShortName => "rg";
        public string Description => $"Send a global chat message after a delay or at a time. Usage: /{CMD} [in hh:mm:ss | at HH:mm] [message]";

        public void Execute(string[] args)
        {
            if ( RemindPlugin.ScheduledTaskManager == null)
            {
                ChatUtils.AddGlobalNotification("ScheduledTaskManager is not initialized.");
                return;
            }
            
            if (args.Length < 3 || (args[0] != "in" && args[0] != "at"))
            {
                ChatUtils.AddGlobalNotification($"Usage: /{CMD} [in hh:mm:ss | at HH:mm] [message]");
                return;
            }

            string message = string.Join(" ", args, 2, args.Length - 2);
            if (string.IsNullOrWhiteSpace(message))
            {
                ChatUtils.AddGlobalNotification("Please enter a valid message.");
                return;
            }

            if (args[0] == "in")
            {
                if (!RemindPlugin.ScheduledTaskManager.TryScheduleIn(args[1], () =>
                    ChatUtils.SendMessageAsync(RemindPlugin.ModName, message, Islocal: false), out _, out string error))
                {
                    ChatUtils.AddGlobalNotification(error);
                    return;
                }
                TimeSpan.TryParse(args[1], out TimeSpan delay);
                ChatUtils.AddGlobalNotification($"[{CMD}] Reminder in {delay:hh\\:mm\\:ss}: \"{message}\"");
            }
            else
            {
                if (!RemindPlugin.ScheduledTaskManager.TryScheduleAt(args[1], () =>
                    ChatUtils.SendMessageAsync(RemindPlugin.ModName, message, Islocal: false), out _, out string error))
                {
                    ChatUtils.AddGlobalNotification(error);
                    return;
                }
                ChatUtils.AddGlobalNotification($"[{CMD}] Reminder at {args[1]}: \"{message}\"");
            }
        }
    }
}
