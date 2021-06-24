using System.Collections;
using PropHunt.Game.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Chat
{
    public class DebugChatUpdate : MonoBehaviour, IScreenComponent
    {
        public Text text;
        public Scrollbar bar;

        public void OnScreenLoaded()
        {
            UpdateText();
            DebugChatLog.DebugChatEvents += HandleEvent;
        }

        public void OnScreenUnloaded()
        {

        }

        public void OnDisable()
        {
            DebugChatLog.DebugChatEvents -= HandleEvent;
        }

        public void HandleEvent(object sender, ChatMessageEvent eventArgs)
        {
            UpdateText();
            bar.value = 0;
        }

        public void UpdateText()
        {
            text.text = DebugChatLog.GetChatLog();
        }
    }
}