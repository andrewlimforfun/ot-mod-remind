using Remind;
using Remind.Core.Util;

namespace Remind.Core.Commands
{
    public class RemindBroadcastCreationCommand : IChatCommand
    {
        public const string CMD = "remindbroadcastcreation";
        public string Name => CMD;
        public string ShortName => "rbc";
        public string Description => "Toggle Remind broadcast creation on/off. ";

        public void Execute(string[] args)
        {
            if (RemindPlugin.BroadcastCreation == null)
            {
                return;
            }

            RemindPlugin.BroadcastCreation.Value = !RemindPlugin.BroadcastCreation.Value;
            ChatUtils.AddGlobalNotification($"Remind broadcast creation is now {(RemindPlugin.BroadcastCreation.Value ? "enabled" : "disabled")}.");
        }
    }
}
