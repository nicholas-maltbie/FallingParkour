using System;
using NUnit.Framework;
using PropHunt.Game.Communication;
using PropHunt.UI.Chat;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for debug chat
    /// </summary>
    [TestFixture]
    public class DebugChatTests
    {
        private GameObject debugChat;

        private DebugChatUpdate chatUpdate;

        private Text text;

        [SetUp]
        public void Setup()
        {
            debugChat = new GameObject();
            chatUpdate = debugChat.AddComponent<DebugChatUpdate>();
            text = debugChat.AddComponent<Text>();
            chatUpdate.text = text;
            chatUpdate.bar = debugChat.AddComponent<Scrollbar>();
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup game object
            GameObject.DestroyImmediate(this.debugChat);
        }

        [Test]
        public void TestDebugChatHandleMessages()
        {
            DebugChatLog.AddLocalMessage(new ChatMessage("source", "test"));
            chatUpdate.OnEnable();
            Assert.IsTrue(text.text == DebugChatLog.GetChatLog());
            DebugChatLog.ClearChatLog();
            Assert.IsTrue(text.text == "");
            DebugChatLog.OnMessage(new ChatMessage("", "test2"));
            Assert.IsTrue(text.text == DebugChatLog.GetChatLog());
            chatUpdate.OnDisable();
            DebugChatLog.AddLocalMessage(new ChatMessage("source2", "test2"));
            Assert.IsTrue(text.text != DebugChatLog.GetChatLog());
        }

        [Test]
        public void TestChatMessageEquality()
        {
            ChatMessage m1 = new ChatMessage("source", "test", DateTime.FromBinary(0));
            ChatMessage m1Other = new ChatMessage("source", "test", DateTime.FromBinary(0));
            Assert.IsTrue(m1.Equals(m1Other));
            ChatMessage m2 = new ChatMessage("source2", "content2");
            Assert.IsFalse(m2.Equals(m1));
        }
    }
}