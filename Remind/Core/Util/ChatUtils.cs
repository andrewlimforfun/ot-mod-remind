using System;
using System.Text;
using System.Text.RegularExpressions;
using Remind;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Remind.Core.Util
{

    public static class ChatUtils
    {
        private static readonly Regex _commonTMPTagRegex = new Regex(
        @"</?(?:color|b|i|u|s|sup|sub|size|alpha|mark|uppercase|lowercase|smallcaps|font|voffset|nobr|noparse|sprite|link|align|rotate|#[0-9a-fA-F]{3,8})[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void AddGlobalNotification(string text)
        {
            TextChannelManager textChannelManager = NetworkSingleton<TextChannelManager>.I;
            if (textChannelManager == null)
            {
                return;
            }
            textChannelManager.AddNotification(text);
        }

        public static void SendMessageAsync(string userName, string text, bool Islocal = false)
        {
            TextChannelManager textChannelManager = NetworkSingleton<TextChannelManager>.I;
            Transform mainPlayer = textChannelManager.MainPlayer;
            byte[] messageBytes = Encoding.Unicode.GetBytes(text[..Math.Min(text.Length, 250)]);
            byte[] userNameBytes = Encoding.Unicode.GetBytes(userName);

            var steamPlayerId = PlayerUtils.GetSteamPlayerIdString();
            textChannelManager.SendMessageAsync(messageBytes, userNameBytes, Islocal, mainPlayer.position, steamPlayerId);
        }

        public static void CleanCommand()
        {
            // These cleanup patterns follow TextChannelManager.OnEnterPressed logic
            // They will hide the command from the chat instead of sending them to the server

            // Synchronizes the task system with the music state - preventing conflicting gameplay inputs during rhythm/music sections
            LockState lockState = NetworkSingleton<MusicManager>.I.IsActive ? LockState.Music : LockState.Free;
            MonoSingleton<TaskManager>.I.SetLockState(lockState);

            // Removes focus from any currently selected UI element        
            EventSystem.current.SetSelectedGameObject(null);

            // special exception for /help since this command is used by other plugins
            var text = MonoSingleton<UIManager>.I.MessageInput.text;
            if (!text.StartsWith("/help") || text == "/help remind")
            {
                // Clears the text input field
                MonoSingleton<UIManager>.I.MessageInput.text = "";
            }
        }

        /// <summary>
        /// Removes common TextMeshPro tags from a string to improve readability when relaying messages from external sources that may contain formatting tags.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanTMPTags(string input)
        {
            // Remove common TMP tags
            return _commonTMPTagRegex.Replace(input, string.Empty);
        }

        /// <summary>
        /// Injects <paramref name="text"/> into the in-game chat input field and submits
        /// it as if the user had typed it, allowing it to be processed by the game's chat system and other mods' command handlers.
        /// </summary>
        public static void UISendMessage(string text)
        {
            var inputField = MonoSingleton<UIManager>.I.MessageInput;
            inputField.text = text;
            inputField.onSubmit.Invoke(inputField.text);
        }

        /// <summary>
        /// Routes an incoming text string (from an external source such as WebSocket or Telegram)
        /// into the appropriate game channel:
        /// <list type="bullet">
        ///   <item>Empty / whitespace — ignored.</item>
        ///   <item><c>/remind*</c> commands — processed directly by <see cref="RemindPlugin.CommandManager"/>.</item>
        ///   <item>Other <c>/</c> commands — injected via the UI input system on the main thread so other mods can handle them.</item>
        ///   <item>Plain text — sent as a global chat message from the local player.</item>
        /// </list>
        /// </summary>
        public static void DispatchIncomingText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            text = text.Trim();

            if (text.StartsWith("/"))
            {
                // Other mods' commands must be injected through the input system to be processed
                RemindPlugin.RunOnMainThread(() =>
                {
                    if (text.StartsWith("/remind"))
                    {
                        bool isProcessed = RemindPlugin.CommandManager?.ProcessInput(text) ?? false;
                        if (isProcessed) return;
                    }
                    // for other commands
                    UISendMessage(text);
                });
            }
            else
            {
                // Normal messages go through the regular chat pipeline as the local player
                string senderName = PlayerUtils.GetUserName();
                SendMessageAsync(senderName, text, Islocal: false);
            }
        }
    }

}
