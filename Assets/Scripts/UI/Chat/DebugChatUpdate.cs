using System.Collections;
using PropHunt.Game.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Chat
{
    public class DebugChatUpdate : MonoBehaviour
    {
        public Text text;
        public Scrollbar bar;

        public void OnEnable()
        {
            UpdateText();
            DebugChatLog.DebugChatEvents += HandleEvent;
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