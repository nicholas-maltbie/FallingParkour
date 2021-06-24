using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Chat
{
    public class ChatBoxAutoScroll : MonoBehaviour, IScreenComponent
    {
        public static float scrollPosition;

        public Scrollbar textScroll;

        public void Awake()
        {
            ChatBoxAutoScroll.scrollPosition = textScroll.value;
        }

        public void OnScrollEvent(float value)
        {
            ChatBoxAutoScroll.scrollPosition = value;
        }

        public IEnumerator SetScroll(float position)
        {
            yield return null;
            textScroll.value = position;
            yield return null;
        }

        public void OnScreenLoaded()
        {
            StartCoroutine(SetScroll(ChatBoxAutoScroll.scrollPosition));
        }

        public void OnScreenUnloaded()
        {

        }
    }
}