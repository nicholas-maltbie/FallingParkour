using Mirror;
using PropHunt.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Game.Communication
{
    /// <summary>
    /// Events for logging to chat information
    /// </summary>
    public readonly struct ChatMessage : IEquatable<ChatMessage>, NetworkMessage
    {
        /// <summary>
        /// Label of source for event as a string
        /// </summary>
        public readonly string source;
        /// <summary>
        /// Time that the event ocurred
        /// </summary>
        public readonly long time;
        /// <summary>
        /// Content int he message
        /// </summary>
        public readonly string content;

        public bool Equals(ChatMessage other)
        {
            return other.source == source && other.time == time && other.content == content;
        }

        public ChatMessage(string source, string content) : this(source, content, DateTime.Now) { }

        public ChatMessage(string source, string content, DateTime time)
        {
            this.source = source;
            this.content = content;
            this.time = time.ToBinary();
        }

        public override string ToString()
        {
            if (source != null && source.Length > 0)
            {
                return $"[{DateTime.FromBinary(time).ToShortTimeString()}] {source}> {content}";
            }
            else
            {
                return $"[{DateTime.FromBinary(time).ToShortTimeString()}] {content}";
            }
        }
    }

    public class ChatMessageEvent : EventArgs
    {
        public ChatMessage message;
    }


    public static class DebugChatLog
    {
        private static List<ChatMessage> messages = new List<ChatMessage>();

        public static event EventHandler<ChatMessageEvent> DebugChatEvents;

        public static string GetChatLog()
        {
            string log = "";
            foreach (ChatMessage chatMessage in messages)
            {
                log += chatMessage.ToString() + "\n";
            }
            return log;
        }

        public static void ClearChatLog()
        {
            messages.Clear();
            DebugChatEvents?.Invoke(null, new ChatMessageEvent { message = new ChatMessage() });
        }

        public static void SendChatMessage(ChatMessage chatMessage)
        {
            AdjustMessageLogServer(chatMessage);
        }

        private static void AdjustMessageLogServer(ChatMessage chatMessage)
        {
            // OnMessage(chatMessage);
            NetworkServer.SendToAll(chatMessage);
        }

        public static void AddInfoMessage(string message)
        {
            AddLocalMessage(new ChatMessage("", message));
        }

        public static void AddLocalMessage(ChatMessage chatMessage)
        {
            messages.Add(chatMessage);
            DebugChatEvents?.Invoke(null, new ChatMessageEvent { message = chatMessage });
        }

        public static void OnMessage(ChatMessage chatMessage)
        {
            AddLocalMessage(chatMessage);
        }
    }
}