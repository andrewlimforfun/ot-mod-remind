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
        
        [HarmonyPatch("AddNotification", typeof(string))]
        [HarmonyPostfix]
        public static void AddNotificationPostfix(string text)
        {
            if (RemindPlugin.EnableFeature?.Value == false) return;

            _ = Task.Run(async () =>
            {
                // always clean text because notifications contain usernames
                string cleanText = ChatUtils.CleanTMPTags(text);
            });
        }

        [HarmonyPatch("SendMessageAsync", typeof(byte[]), typeof(byte[]), typeof(bool), typeof(Vector3), typeof(string), typeof(RPCInfo))]
        [HarmonyPostfix]
        public static void SendMessageAsyncPostfix(byte[] textBytes, byte[] userName, bool isLocal, Vector3 pos, string playerID, RPCInfo info = default(RPCInfo))
        {
            if (RemindPlugin.EnableFeature?.Value == false) return;

            _ = Task.Run(async () =>
            {
                string channel = isLocal ? "Local" : "Global";
                // always clean username - hard to read otherwise
                string cleanUserName = ChatUtils.CleanTMPTags(Encoding.Unicode.GetString(userName));
                string rawMessage = Encoding.Unicode.GetString(textBytes);
                string cleanMessage = RemindPlugin.CleanChatSinkTags?.Value == true
                    ? ChatUtils.CleanTMPTags(rawMessage)
                    : rawMessage;

            });
        }

        [HarmonyPatch("OnChannelMessageReceived", typeof(string), typeof(string), typeof(Vector3), typeof(bool), typeof(int), typeof(string))]
        [HarmonyPostfix]
        public static void OnChannelMessageReceivedPostfix(string userName, string message, Vector3 senderPosition, bool isLocal, int senderIndex, string playerID)
        {
            if (RemindPlugin.EnableFeature?.Value == false) return;

            if (MonoSingleton<DataManager>.I.BanData.IgnorePlayers.Contains(playerID)) return;
            if (MonoSingleton<DataManager>.I.BanData.MutedPlayers.Contains(playerID)) return;

            if (PlayerUtils.GetSteamPlayerIdString() == playerID) return;

            // fire-and-forget task to avoid hitching the main thread with file IO / WebSocket work
            _ = Task.Run(async () =>
            {
                // filter by distance for local messages and include it in the channel label
                string channel = isLocal ? "Local" : "Global";

                int? distance = null;
                if (isLocal)
                {
                    distance = (int) Vector3.Distance(senderPosition, NetworkSingleton<TextChannelManager>.I.MainPlayer.position);
                    int localRange = RemindPlugin.ChatSinkLocalRange?.Value ?? RemindPlugin.DefaultChatSinkLocalRange;
                    if (distance > localRange) return;
                }

                // always clean username - hard to read otherwise
                string cleanUserName = ChatUtils.CleanTMPTags(userName);
                string cleanMessage = RemindPlugin.CleanChatSinkTags?.Value == true
                    ? ChatUtils.CleanTMPTags(message)
                    : message;

            });
        }

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
