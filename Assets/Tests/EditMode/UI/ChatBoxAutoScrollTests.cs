using System;
using System.Collections;
using NUnit.Framework;
using PropHunt.Game.Communication;
using PropHunt.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for chat box auto scroll
    /// </summary>
    [TestFixture]
    public class ChatBoxAutoScrollTests
    {
        private GameObject go;

        private ChatBoxAutoScroll chatBoxAuto;

        private Scrollbar scrollbar;

        [SetUp]
        public void Setup()
        {
            this.go = new GameObject();
            this.chatBoxAuto = go.AddComponent<ChatBoxAutoScroll>();
            this.scrollbar = go.AddComponent<Scrollbar>();
            this.chatBoxAuto.textScroll = scrollbar;
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup game object
            GameObject.DestroyImmediate(this.go);
        }

        [UnityTest]
        public IEnumerator TestChatBoxAutoScroll()
        {
            this.scrollbar.value = 0.5f;
            this.chatBoxAuto.Awake();
            Assert.IsTrue(ChatBoxAutoScroll.scrollPosition == 0.5f);
            this.chatBoxAuto.OnScrollEvent(0.0f);
            Assert.IsTrue(ChatBoxAutoScroll.scrollPosition == 0.0f);


            IEnumerator enumerator = this.chatBoxAuto.SetScroll(0.8f);
            while (enumerator.MoveNext()) { };
            Assert.IsTrue(scrollbar.value == 0.8f);

            ChatBoxAutoScroll.scrollPosition = 1.0f;
            this.chatBoxAuto.OnEnable();
            yield return null;
            // yield return null;
            // Assert.IsTrue(scrollbar.value == 1.0f);
        }
    }
}