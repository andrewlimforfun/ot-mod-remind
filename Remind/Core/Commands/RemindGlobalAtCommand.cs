using System;
using System.Xml;
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

            if (!DateTime.TryParse(timePart, out DateTime time))
            {
                ChatUtils.AddGlobalNotification("Invalid time format. Please use HH:mm.");
                return;
            }

            ChatUtils.AddGlobalNotification($"[{CMD}] Reminder at {timePart}: \"{message}\"");

            TimeSpan delay = time - DateTime.Now;
            string delayStr = XmlConvert.ToString(delay).Replace("PT", "").Replace("H", "h ").Replace("M", "m ").Replace("S", "s").Trim();
            ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, $"in {delayStr} with '{message}'", Islocal: false);
            RemindPlugin.ScheduledTaskManager.ScheduleAt(time, 
                () => ChatUtils.SendMessageAsync(RemindPlugin.ModName + username, message, Islocal: false));
            
        }
    }
}
