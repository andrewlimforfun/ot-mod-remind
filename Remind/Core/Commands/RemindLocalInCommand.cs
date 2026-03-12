using System;
using Remind;
using Remind.Core.Util;

namespace Remind.Core.Commands
{
    public class RemindLocalInCommand : IChatCommand
    {
        public const string CMD = "remindlocalin";
        public string Name => CMD;
        public string ShortName => "rli";
        public string Description => $"Send a local chat message after a delay. Usage: /{CMD} [hh:mm:ss] [message]";

        public void Execute(string[] args)
        {
            if (RemindPlugin.ScheduledTaskManager == null)
            {
                ChatUtils.AddGlobalNotification("ScheduledTaskManager is not initialized.");
                return;
            }

            if (args.Length < 2)
            {
                ChatUtils.AddGlobalNotification($"Usage: /{CMD} [hh:mm:ss] [message]");
                return;
            }

            string message = string.Join(" ", args, 1, args.Length - 1);
            if (string.IsNullOrWhiteSpace(message))
            {
                ChatUtils.AddGlobalNotification("Please enter a valid message.");
                return;
            }

            string timePart = args[0];
            string username = PlayerUtils.GetUserName();
            if (!RemindPlugin.ScheduledTaskManager.TryScheduleIn(args[0], () =>
                ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, message, Islocal: true), out _, out string error))
            {
                ChatUtils.AddGlobalNotification(error);
                return;
            }
            ChatUtils.AddGlobalNotification($"[{CMD}] Reminder in {timePart}: \"{message}\"");
            ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, $"{username} in {timePart}: ", Islocal: true);
        }
    }
}
