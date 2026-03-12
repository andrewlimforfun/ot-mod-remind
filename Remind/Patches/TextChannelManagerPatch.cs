using HarmonyLib;
using UnityEngine;
using Remind.Core;
using Remind.Core.Commands;
using System;
using System.Threading.Tasks;
using PurrNet;
using System.Text;
using Remind.Core.Util;

namespace Remind.Patches
{
    [HarmonyPatch(typeof(TextChannelManager))]

    public class TextChannelManagerPatch
    {
        const int _selfDistance = 0;

        // [HarmonyPatch("AddNotification", typeof(string))]
        // [HarmonyPostfix]
        // public static void AddNotificationPostfix(string text)
        // {
        // }

        // [HarmonyPatch("SendMessageAsync", typeof(byte[]), typeof(byte[]), typeof(bool), typeof(Vector3), typeof(string), typeof(RPCInfo))]
        // [HarmonyPostfix]
        // public static void SendMessageAsyncPostfix(byte[] textBytes, byte[] userName, bool isLocal, Vector3 pos, string playerID, RPCInfo info = default(RPCInfo)) { }

        // [HarmonyPatch("OnChannelMessageReceived", typeof(string), typeof(string), typeof(Vector3), typeof(bool), typeof(int), typeof(string))]
        // [HarmonyPostfix]
        // public static void OnChannelMessageReceivedPostfix(string userName, string message, Vector3 senderPosition, bool isLocal, int senderIndex, string playerID){}

        [HarmonyPatch("OnEnterPressed")]
        [HarmonyPrefix]
        public static void OnEnterPressedPrefix()
        {
            string text = MonoSingleton<UIManager>.I.MessageInput.text;

            // basic validation to only process potential commands - this will allow normal chat messages to go through without interference
            if (string.IsNullOrEmpty(text) || text.StartsWith('/') == false || RemindPlugin.EnableFeature == null)
            {
                return;
            }

            // process commands only if feature is enabled or its a feature toggle command, 
            // so that users can still toggle the feature on if they have it off
            // unrecognized commands will be ignored and treated as normal chat messages
            if (RemindPlugin.EnableFeature.Value == true || text.Contains(RemindToggleCommand.CMD))
            {
                bool isProcessed = RemindPlugin.CommandManager?.ProcessInput(text) ?? false;

                // only clean the command from chat if the ShowCommand option is disabled
                // otherwise the lock management and unselection can happen in the real OnEnterPressed
                if (RemindPlugin.ShowCommand?.Value == false && isProcessed)
                {
                    ChatUtils.CleanCommand();
                }
            }

        }
    }
}
