using Remind;
using Remind.Core.Util;

namespace Remind.Core.Commands
{
    public class RemindGlobalAtCommand : IChatCommand
    {
        public const string CMD = "remindglobalat";
        public string Name => CMD;
        public string ShortName => "rga";
        public string Description => $"Send a global chat message at a specific time. Usage: /{CMD} [HH:mm] [message]";

        public void Execute(string[] args)
        {
            if (RemindPlugin.ScheduledTaskManager == null)
            {
                ChatUtils.AddGlobalNotification("ScheduledTaskManager is not initialized.");
                return;
            }

            if (args.Length < 2)
            {
                ChatUtils.AddGlobalNotification($"Usage: /{CMD} [HH:mm] [message]");
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

            if (!RemindPlugin.ScheduledTaskManager.TryScheduleAt(timePart, () =>
                ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, message, Islocal: false), out _, out string error))
            {
                ChatUtils.AddGlobalNotification(error);
                return;
            }
            ChatUtils.AddGlobalNotification($"[{CMD}] Reminder at {timePart}: \"{message}\"");
            ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, $"at {timePart} with '{message}'", Islocal: false);
        }
    }
}
