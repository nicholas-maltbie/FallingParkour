using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Code based on a nice tutorial provided by Mark Hedberg via their blog
// https://www.hedberggames.com/blog/creating-ui-tabs-in-unity-part-1

namespace PropHunt.UI
{
    [Serializable]
    public class TabPair
    {
        public Button TabButton;
        public CanvasGroup TabContent;
    }

    public class TabStrip : MonoBehaviour
    {
        public TabPair[] TabCollection;
        public Sprite TabIconPicked;
        public Sprite TabIconDefault;
        public Button DefaultTab;

        public int CurrentTabIndex { get; private set; }

        protected void SetTabState(int index, bool picked)
        {
            TabPair affectedItem = TabCollection[index];
            affectedItem.TabContent.interactable = picked;
            affectedItem.TabContent.blocksRaycasts = picked;
            affectedItem.TabContent.alpha = picked ? 1 : 0;
            affectedItem.TabButton.image.sprite = picked ? TabIconPicked : TabIconDefault;
        }

        public void PickTab(int index)
        {
            SetTabState(CurrentTabIndex, false);
            CurrentTabIndex = index;
            SetTabState(CurrentTabIndex, true);
        }

        protected int? FindTabIndex(Button tabButton)
        {
            var currentTabPair = TabCollection.FirstOrDefault(x => x.TabButton == tabButton);
            if (currentTabPair == default)
            {
                Debug.LogWarning("The tab " + DefaultTab.gameObject.name + " does not belong to the tab strip " + name + ".");
                return null;
            }
            return Array.IndexOf(TabCollection, currentTabPair);
        }

        public void Start()
        {
            for (var i = 0; i < TabCollection.Length; i++)
            {
                //Storing the current value of i in a locally scoped variable.
                var index = i;
                TabCollection[index].TabButton.onClick.AddListener(new UnityAction(() => PickTab(index)));
            }
            //Initialize all tabs to an unpicked state
            for (var i = 0; i < TabCollection.Length; i++)
            {
                SetTabState(i, false);
            }
            //Pick the default tab
            if (TabCollection.Length > 0)
            {
                var index = FindTabIndex(DefaultTab);
                //If tab is invalid, instead default to the first tab.
                if (index == null)
                    index = 0;
                CurrentTabIndex = index.Value;
                SetTabState(CurrentTabIndex, true);
            }
        }
    }
}